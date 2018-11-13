/*
 * User: Joshua
 * Date: 4/08/2016
 * Time: 6:19 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Common byte encodings for types of data.
  /// </summary>
  public static class Encodings {
    
    //-------------------------------------------//
    
    /// <summary>
    /// UTF-8 text encoding
    /// </summary>
    public static byte[] Utf8 = { 239, 187, 191 };
    
    /// <summary>
    /// UCS-2 Little Endian
    /// </summary>
    public static byte[] Ucs2 = { 255, 254 };
    
    /// <summary>
    /// TBI - Windows Disk Image file
    /// </summary>
    public static byte[] Tbi = { 0, 0, 0, 0, 20, 0, 0, 0 };
    
    /// <summary>
    /// [8 byte offset]
    /// DAT - Bitcoin Core wallet.dat file
    /// </summary>
    public static byte[] DatBitcoin = { 0, 0, 0, 0, 98, 49, 5, 0, 9, 0, 0, 0, 0, 32, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0 };
    
    /// <summary>
    /// JP2 - Various JPEG-2000 image file formats
    /// </summary>
    public static byte[] Jp2 = { 0, 0, 0, 12, 106, 80, 32, 32, 13, 10 };
    
    /// <summary>
    /// 3GG, 3GP, 3G2 - 3rd Generation Partnership Project 3GPP multimedia files
    /// </summary>
    public static byte[] ThreeGp1 = { 0, 0, 0, 20, 102, 116, 121, 112, 51, 103, 112 };
    
    /// <summary>
    /// 3GG, 3GP, 3G2 - 3rd Generation Partnership Project 3GPP2 multimedia files
    /// </summary>
    public static byte[] ThreeGp2 = { 0, 0, 0, 32, 102, 116, 121, 112, 51, 103, 112 };
    
    /// <summary>
    /// MP4 - ISO Base Media file (MPEG-4) v1
    /// </summary>
    public static byte[] Mp4IsoMedia = { 0, 0, 0, 20, 102, 116, 121, 112, 105, 115, 111, 109 };
    
    /// <summary>
    /// MOV - QuickTime movie file
    /// </summary>
    public static byte[] Mov1 = { 0, 0, 0, 20, 102, 116, 121, 112, 113, 116, 32, 32 };
    /// <summary>
    /// [4 byte offset]
    /// MOV - QuickTime movie file
    /// </summary>
    public static byte[] Mov2 = { 102, 116, 121, 112, 113, 116, 32, 32 };
    /// <summary>
    /// [4 byte offset]
    /// MOV - QuickTime movie file
    /// </summary>
    public static byte[] Mov3 = { 109, 111, 111, 118 };
    
    /// <summary>
    /// M4V - MPEG-4 video/QuickTime file
    /// </summary>
    public static byte[] M4v1 = { 0, 0, 0, 24, 102, 116, 121, 112, 109, 112, 52, 50 };
    /// <summary>
    /// [4 byte offset]
    /// M4V - MPEG-4 video|QuickTime file
    /// </summary>
    public static byte[] M4v2 = { 102, 116, 121, 112, 109, 112, 52, 50 };
    
    /// <summary>
    /// MP4 - MPEG-4 video files
    /// </summary>
    public static byte[] Mp4_1 = { 0, 0, 0, 24, 102, 116, 121, 112, 51, 103, 112, 53 };
    /// <summary>
    /// MP4 - MPEG-4 video file
    /// </summary>
    public static byte[] Mp4_2 = { 0, 0, 0, 28, 102, 116, 121, 112, 77, 83, 78, 86, 1, 41, 0, 70, 77, 83, 78, 86, 109, 112, 52, 50 };
    /// <summary>
    /// [4 byte offset]
    /// MP4 - MPEG-4 video files
    /// </summary>
    public static byte[] Mp4_3 = { 102, 116, 121, 112, 51, 103, 112, 53 };
    /// <summary>
    /// [4 byte offset]
    /// MP4 - MPEG-4 video file
    /// </summary>
    public static byte[] Mp4_4 = { 102, 116, 121, 112, 77, 83, 78, 86 };
    
    /// <summary>
    /// M4A - Apple Lossless Audio Codec file
    /// </summary>
    public static byte[] M4a1 = { 0, 0, 0, 32, 102, 116, 121, 112, 77, 52, 65, 32 };
    /// <summary>
    /// [4 byte offset]
    /// M4A - Apple Lossless Audio Codec file
    /// </summary>
    public static byte[] M4a2 = { 102, 116, 121, 112, 77, 52, 65, 32 };
    
    /// <summary>
    /// ICO - Windows icon file
    /// </summary>
    public static byte[] Ico = { 0, 0, 1, 0 };
    
    /// <summary>
    /// CUR - Windows cursor file
    /// </summary>
    public static byte[] Cur = { 0, 0, 2, 0 };
    
    /// <summary>
    /// TTF - TrueType font file
    /// </summary>
    public static byte[] Ttf = { 0, 1, 0, 0, 0 };
    
    /// <summary>
    /// MDF - Microsoft SQL Server 2000 database
    /// </summary>
    public static byte[] Mdf = { 1, 16, 0, 0 };
    
    /// <summary>
    /// WEBM - WebM video file
    /// </summary>
    public static byte[] Webm = { 26, 69, 223, 163 };
    
    /// <summary>
    /// GZ, TGZ, GZIP - Archive file
    /// VLT - VLC Player Skin file
    /// </summary>
    public static byte[] Gzip = { 31, 139, 8 };
    
    /// <summary>
    /// TAR.Z - Compressed tape archive file using standard (Lempel-Ziv-Welch) compression
    /// </summary>
    public static byte[] Tar = { 31, 157 };
    
    /// <summary>
    /// TAR.Z - Compressed tape archive file using LZH (Lempel-Ziv-Huffman) compression
    /// </summary>
    public static byte[] TarLzh = { 31, 160 };
    
    /// <summary>
    /// PDF - Adobe Portable Document Format
    /// FDF - Forms Document Format
    /// AI - Illustrator graphics files
    /// </summary>
    public static byte[] Pdf = { 37, 80, 68, 70 };
    
    /// <summary>
    /// PDF, FDF, AI - Potential trailing bytes
    /// </summary>
    public static byte[] PdfTrail1 = { 10, 37, 37, 69, 79, 70 };
    /// <summary>
    /// PDF, FDF, AI - Potential trailing bytes
    /// </summary>
    public static byte[] PdfTrail2 = { 10, 37, 37, 69, 79, 70, 10 };
    /// <summary>
    /// PDF, FDF, AI - Potential trailing bytes
    /// </summary>
    public static byte[] PdfTrail3 = { 13, 10, 37, 37, 69, 79, 70, 13, 10 };
    /// <summary>
    /// PDF, FDF, AI - Potential trailing bytes
    /// </summary>
    public static byte[] PdfTrail4 = { 13, 37, 37, 69, 79, 70, 13 };
    
    /// <summary>
    /// FBM - Fuzzy bitmap (FBM) file
    /// </summary>
    public static byte[] Fbm = { 37, 98, 105, 116, 109, 97, 112 };
    
    /// <summary>
    /// LHA, LZH - Compressed archive file
    /// </summary>
    public static byte[] Lha = { 45, 108, 104 };
    
    /// <summary>
    /// IVR - RealPlayer video file (V11 and later)
    /// </summary>
    public static byte[] Ivr = { 46, 82, 69, 67 };
    
    /// <summary>
    /// RM, RMVB - RealMedia streaming media file
    /// </summary>
    public static byte[] Rm = { 46, 82, 77, 70 };
    
    /// <summary>
    /// RA - RealAudio file
    /// </summary>
    public static byte[] Ra = { 46, 82, 77, 70, 0, 0, 0, 18, 0 };
    
    /// <summary>
    /// RA - RealAudio streaming media file
    /// </summary>
    public static byte[] RaStream = { 46, 114, 97, 253, 0 };
    
    /// <summary>
    /// ASF, WMA, WMV - Microsoft Windows Media Audio/Video File
    /// </summary>
    public static byte[] Wmv = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
    
    /// <summary>
    /// BMP, DIB - Windows (or device-independent) bitmap image
    /// NOTE: Byte indices 2-5 contain the file length in little-endian order.
    /// </summary>
    public static byte[] Bmp = { 66, 77 };
    
    /// <summary>
    /// BPG - Better Portable Graphics image format
    /// </summary>
    public static byte[] Bpg = { 66, 80, 71, 251 };
    
    /// <summary>
    /// BZ2, TAR.BZ2, TBZ2, TB2 - bzip2 compressed archive
    /// </summary>
    public static byte[] Bz2 = { 66, 90, 104 };
    
    /// <summary>
    /// DST - DST compressed file
    /// </summary>
    public static byte[] Dst = { 68, 83, 84, 98 };
    
    /// <summary>
    /// FLV - Flash video file
    /// </summary>
    public static byte[] Flv = { 70, 76, 86, 1 };
    
    /// <summary>
    /// GIF - Graphics interchange format file
    /// NOTE - last two bytes are trailing bytes
    /// </summary>
    public static byte[] Gif1 = { 71, 73, 70, 56, 55, 97, 0, 59 };
    /// <summary>
    /// GIF - Graphics interchange format file
    /// NOTE - last two bytes are trailing bytes
    /// </summary>
    public static byte[] Gif2 = { 71, 73, 70, 56, 57, 97, 0, 59 };
    
    /// <summary>
    /// TIF, TIFF - Tagged Image File Format file
    /// </summary>
    public static byte[] Tif = { 73, 32, 73 };
    
    /// <summary>
    /// MP3 - MPEG-1 Audio Layer 3 (MP3) audio file
    /// </summary>
    public static byte[] Mp3 = { 73, 68, 51 };
    
    /// <summary>
    /// Ogg - Open multimedia container format.
    /// </summary>
    public static byte[] Ogg = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
    
    /// <summary>
    /// JAR - Java archive
    /// </summary>
    public static byte[] Jar = { 80, 75, 3, 4, 20, 0, 8, 0, 8, 0 };
    
    /// <summary>
    /// AVI - Resource Interchange File Format -- Windows Audio Video Interleave file, where '255, 255, 255, 255' is the file size (little endian)
    /// </summary>
    public static byte[] Avi = { 82, 73, 70, 70, 255, 255, 255, 255, 65, 86, 73, 32, 76, 73, 83, 84 };
    
    /// <summary>
    /// WAV - Resource Interchange File Format -- Audio for Windows file, where '255, 255, 255, 255' is the file size (little endian)
    /// </summary>
    public static byte[] Wav = { 82, 73, 70, 70, 255, 255, 255, 255, 87, 65, 86, 69, 102, 109, 116, 32 };
    
    /// <summary>
    /// FLAC - Free Lossless Audio Codec file
    /// </summary>
    public static byte[] Flac = { 102, 76, 97, 67, 0, 0, 0, 34 };
    
    /// <summary>
    /// RTF - Rich text format word processing file
    /// NOTE : Trailer is the last 7 bytes
    /// </summary>
    public static byte[] Rtf = { 123, 92, 114, 116, 102, 49, 92, 112, 97, 114, 32, 125, 125 };
    
    /// <summary>
    /// PNG - Portable Network Graphics file
    /// NOTE : The last 8 bytes are trailing.
    /// </summary>
    public static byte[] Png = { 137, 80, 78, 71, 13, 10, 26, 10, 73, 69, 78, 68, 174, 66, 96, 130 };
    
    /// <summary>
    /// JFIF, JPE, JPEG, JPG, JPEG/JFIF - graphics file
    /// NOTE - byte indices 3, 4 and 5 are misc byte and the last two bytes are trailing bytes
    /// </summary>
    public static byte[] Jpg = { 255, 216, 255, 0, 0, 0, 74, 70, 73, 70, 0, 255, 217 };
    
    /// <summary>
    /// AAC - MPEG-4 Advanced Audio Coding (AAC) Low Complexity (LC) audio file
    /// </summary>
    public static byte[] Aac1 = { 255, 241 };
    /// <summary>
    /// AAC - MPEG-2 Advanced Audio Coding (AAC) Low Complexity (LC) audio file
    /// </summary>
    public static byte[] Aac2 = { 255, 249 };
    
    /// <summary>
    /// Torrent - Bit-torrent file.
    /// </summary>
    public static byte[] Torrent = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
  
  
  
}
