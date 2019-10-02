# MultiCipher for KeePass
This is a dual cipher model with independent keys that uses two ciphers to encrypt the KeePass database doubling the data length with random pad. It follows a method as mentioned by Bruce Schneier in his book "Applied Cryptography".


## Version 2.1 for KeePass >= 2.41
MultiCipher now supports Yubikey HMAC-SHA1 Challenge/Response

**_Please note:_**
The plugin requires version 2.41 of KeePass and will not work with older version of KeePass, however you may open an older version of the MultiCipher encoded KeePass database which will result in an upgrade to this version.

#### Version 2.1 Data Format
* ___n...___ = Plain Text
* ___n___ = Plain Text Length in bytes
* ___b___ = Block pad length in bytes = `64` - ___n___ % `64` (but `0` if ___n___ % `64` is `0`)
* ___b...___ = Unknown bytes of ___b___ length
* ___nb...___ = Block aligned plain text = ___n...___ + ___b...___
* ___bl___ = Block length in bytes = ___n___ + ___b___
* ___mb...___ = Random bytes of length ___bl___
* ___C___ = Cipher
* ___i2___ = IV Length in bytes of 2nd Cipher
* ___M2___ = 32 byte Master seed for 2nd cipher
* ___S2___ = 32 byte Random seed for 2nd cipher
* ___SR___ = Number of Transformation rounds for Key Derivation Function
* ___K1___ = Master Composite Key provided by KeePass
* ___K2___ = 2nd Composite Key as described below
* ___K2Opt___ = Composite Key Generation Option
* ___Ch___ = Yubikey HMAC-SHA1 Challenge (Randomly Generated)


Position|Length (Bytes)|Content
----|----|-----
0x00|0x01|File Version (`2`)
0x01|0x01|Sub Version (`1`)
0x02|0x01|___C___ (First Cipher)
0x03|0x01|___C___ (Second Cipher)
0x04|0x01|___K2Opt___ (See Below)

**If ___K2Opt___ is 2**

Position|Length (Bytes)|Content
----|----|-----
0x05|0x01|Yubikey Slot (1 or 2)
0x06|0x01|Yubikey HMAC-SHA1 Challenge Length (0x3C or 0x40)
0x07|0x40|___Ch___ Always 64 bytes
0x47|0x01|Key Derivation Method (`1` - AESKDF)
0x48|0x20|___M2___
0x68|0x20|___S2___
0x88|___i2___|Random IV bytes for 2nd Cipher
0x88+___i2___|0x08|___SR___ stored as LittleEndian (defult value `10000` for 2nd Cipher)
0x90+___i2___|0x04|___n___ stored as LittleEndian
0x94+___i2___|___bl___|Cipher 1 applied on ( ___nb...___ XORred with ___mb...___ )
0x94+___i2___+___bl___|___bl___|Cipher 2 applied on ( ___mb...___ )


**If ___K2Opt___ is not 2**

Position|Length (Bytes)|Content
----|----|-----
0x05|0x01|Key Derivation Method (`1` - AESKDF)
0x06|0x20|___M2___
0x26|0x20|___S2___
0x46|___i2___|Random IV bytes for 2nd Cipher
0x46+___i2___|0x08|___SR___ stored as LittleEndian (defult value `10000` for 2nd Cipher)
0x4E+___i2___|0x04|___n___ stored as LittleEndian
0x52+___i2___|___bl___|Cipher 1 applied on ( ___nb...___ XORred with ___mb...___ )
0x52+___i2___+___bl___|___bl___|Cipher 2 applied on ( ___mb...___ )

#### Cipher
**___C___ is defined as**

Byte Value|Algorithm|IV Size (___i2___)
----------|---------|-------
`1`|AES      | 16
`2`|3DES     | 8
`3`|ChaCha20 | 12
`4`|Salsa20  | 8 

### Symmetric Key Derivation

#### First Cipher
 * Use Symmetric Key provided by KeePass
 * Use IV provided by KeePass

#### Second Cipher

**___K2Opt___ is defined as**

Byte Value|___K2___ generated as
----------|-----------
0         | Use a second independent password as a composite key
1         | ___K1___ + Key derived from string literal "TR"
2         | Yubikey HMAC-SHA1 on ___Ch___ + Key derived from string literal "TR"


**Key Generation Steps**
* ___K2___ = As genererated based on ___K2Opt___
* AesKdf = KeePass provided AES Key Derivation Function
* ___KDFResult___ = AesKdf on ___K2___ with ___S2___ and ___SR___ as parameters
* ___XORredPlainTextHash___ = SHA256 of first ___n___ bytes of ( ___nb...___ XORred with ___mb...___ )
* IV is randomly generated
* Symmetric Key is generted by Performing SHA256 on (___M2___ + ___KDFResult___ +  ___XORredPlainTextHash___).

 