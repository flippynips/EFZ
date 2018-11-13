/*
 * User: Joshua
 * Date: 25/06/2016
 * Time: 3:16 PM
 */

namespace Efz {
  /// <summary>
  /// Description of Ascii bytes.
  /// </summary>
  public static class Ascii {
    
    // control
    public const byte Null = 0;
    public const byte StartHeading = 1;
    public const byte StartText = 2;
    public const byte EndText = 3;
    public const byte EndTransmission = 4;
    public const byte Enquiry = 5;
    public const byte Acknowledge = 6;
    public const byte Bell = 7;
    public const byte Backspace = 8;
    public const byte Tab = 9;
    public const byte NewLine = 10;
    public const byte TabVertical = 11;
    public const byte NewPage = 12;
    public const byte CarriageReturn = 13;
    public const byte ShiftOut = 14;
    public const byte ShiftIn = 15;
    public const byte DataLink = 16;
    public const byte Device1 = 17;
    public const byte Device2 = 18;
    public const byte Device3 = 19;
    public const byte Device4 = 20;
    public const byte AcknowledgeNegative = 21;
    public const byte Idle = 22;
    public const byte EndBlock = 23;
    public const byte Cancel = 24;
    public const byte EndMedium = 25;
    public const byte Substitute = 26;
    public const byte Escape = 27;
    public const byte SeparatorFile = 28;
    public const byte SeparatorGroup = 29;
    public const byte SeparatorRecord = 30;
    public const byte SeparatorUnit = 31;
    
    // punctuation and symbols
    public const byte Space = 32;
    public const byte Exclamation = 33;
    public const byte DoubleQuote = 34;
    public const byte Hash = 35;
    public const byte Dollar = 36;
    public const byte Percent = 37;
    public const byte And = 38;
    public const byte Quote = 39;
    public const byte BracketOpen = 40;
    public const byte BracketClose = 41;
    public const byte Star = 42;
    public const byte Plus = 43;
    public const byte Comma = 44;
    public const byte Dash = 45;
    public const byte Stop = 46;
    public const byte SlashForward = 47;
    
    public const byte Colon = 58;
    public const byte SemiColon = 59;
    public const byte LessThan = 60;
    public const byte Equal = 61;
    public const byte GreaterThan = 62;
    public const byte Question = 63;
    public const byte At = 64;
    
    public const byte BracketSqOpen = 91;
    public const byte SlashBack = 92;
    public const byte BracketSqClose = 93;
    public const byte Caret = 94;
    public const byte Underscore = 95;
    public const byte Accent = 96;
    
    public const byte BraceOpen = 123;
    public const byte Bar = 124;
    public const byte BraceClose = 125;
    public const byte Tilde = 126;
    public const byte Delete = 127;
    
    // numbers
    public const byte n0 = 48;
    public const byte n1 = 49;
    public const byte n2 = 50;
    public const byte n3 = 51;
    public const byte n4 = 52;
    public const byte n5 = 53;
    public const byte n6 = 54;
    public const byte n7 = 55;
    public const byte n8 = 56;
    public const byte n9 = 57;
    
    // letters
    public const byte a = 97;
    public const byte b = 98;
    public const byte c = 99;
    public const byte d = 100;
    public const byte e = 101;
    public const byte f = 102;
    public const byte g = 103;
    public const byte h = 104;
    public const byte i = 105;
    public const byte j = 106;
    public const byte k = 107;
    public const byte l = 108;
    public const byte m = 109;
    public const byte n = 110;
    public const byte o = 111;
    public const byte p = 112;
    public const byte q = 113;
    public const byte r = 114;
    public const byte s = 115;
    public const byte t = 116;
    public const byte u = 117;
    public const byte v = 118;
    public const byte w = 119;
    public const byte x = 120;
    public const byte y = 121;
    public const byte z = 122;
    
    public const byte A = 65;
    public const byte B = 66;
    public const byte C = 67;
    public const byte D = 68;
    public const byte E = 69;
    public const byte F = 70;
    public const byte G = 71;
    public const byte H = 72;
    public const byte I = 73;
    public const byte J = 74;
    public const byte K = 75;
    public const byte L = 76;
    public const byte M = 77;
    public const byte N = 78;
    public const byte O = 79;
    public const byte P = 80;
    public const byte Q = 81;
    public const byte R = 82;
    public const byte S = 83;
    public const byte T = 84;
    public const byte U = 85;
    public const byte V = 86;
    public const byte W = 87;
    public const byte X = 88;
    public const byte Y = 89;
    public const byte Z = 90;
    
    // extended
    public const byte CLatinCedil = 128;
    public const byte uLatinDieres = 129;
    public const byte eLatinAcute = 130;
    public const byte aLatinCircum = 131;
    public const byte aLatinDieres = 132;
    public const byte aLatinGrave = 133;
    public const byte aLatinRing = 134;
    public const byte cLatinCedil = 135;
    public const byte eLatinCircum = 136;
    public const byte eLatinDieres = 137;
    public const byte eLatinGrave = 138;
    public const byte iLatinDieres = 139;
    public const byte iLatinCircum = 140;
    public const byte iLatinGrave = 141;
    public const byte ALatinDieres = 142;
    public const byte ALatinRing = 143;
    public const byte ELatinAcute = 144;
    public const byte aeLatinLiga = 145;
    public const byte AELatinLiga = 146;
    public const byte oLatinCircum = 147;
    public const byte oLatinDieres = 148;
    public const byte oLatinGrave = 149;
    public const byte uLatinCircum = 150;
    public const byte uLatinGrave = 151;
    public const byte yLatinDieres = 152;
    public const byte OLatinDieres = 153;
    public const byte ULatinDieres = 154;
    public const byte Cent = 155;
    public const byte Pound = 156;
    public const byte Yen = 157;
    public const byte Peseta = 158;
    public const byte fLatinHook = 159;
    public const byte aLatinAcute = 160;
    public const byte iLatinAcute = 161;
    public const byte oLatinAcute = 162;
    public const byte uLatinAcute = 163;
    public const byte nLatinTilde = 164;
    public const byte NLatinTilde = 165;
    public const byte Feminine = 166;
    public const byte Masculine = 167;
    public const byte QuestionInvert = 168;
    public const byte ReversedNot = 169;
    public const byte Not = 170;
    public const byte Half = 171;
    public const byte Quater = 172;
    public const byte ExclaimInvert = 173;
    public const byte DoubleLeft = 174;
    public const byte DoubleRight = 175;
    public const byte BoxFullLight = 176;
    public const byte BoxFullMedium = 177;
    public const byte BoxFullDark = 178;
    public const byte BoxVert = 179;
    public const byte BoxVertLeft = 180;
    public const byte BoxVertLeftDbl = 181;
    public const byte BoxVertDblLeft = 182;
    public const byte BoxDownDblLeft = 183;
    public const byte BoxDownLeftDbl = 184;
    public const byte BoxVertDblLeftDbl = 185;
    public const byte BoxVertDbl = 186;
    public const byte BoxDownDblLeftDbl = 187;
    public const byte BoxUpDblLeftDbl = 188;
    public const byte BoxUpDblLeft = 189;
    public const byte BoxUpLeftDbl = 190;
    public const byte BoxDownLeft = 191;
    public const byte BoxUpRight = 192;
    public const byte BoxUpHoriz = 193;
    public const byte BoxDownHoriz = 194;
    public const byte BoxVertRight = 195;
    public const byte BoxHoriz = 196;
    public const byte BoxVertHoriz = 197;
    public const byte BoxVertRightDbl = 198;
    public const byte BoxVertDblRight = 199;
    public const byte BoxUpDblRightDbl = 200;
    public const byte BoxDownDblRightDbl = 201;
    public const byte BoxUpDblHorizDbl = 202;
    public const byte BoxDownDblHorizDbl = 203;
    public const byte BoxVertDblRightDbl = 204;
    public const byte BoxHorizDbl = 205;
    public const byte BoxVertDblHorizDbl = 206;
    public const byte BoxUpHorizDbl = 207;
    public const byte BoxUpDblHoriz = 208;
    public const byte BoxDownHorizDbl = 209;
    public const byte BoxDownDblHoriz = 210;
    public const byte BoxUpDblRight = 211;
    public const byte BoxUpRightDbl = 212;
    public const byte BoxDownRightDbl = 213;
    public const byte BoxDownDblRight = 214;
    public const byte BoxVertDblHoriz = 215;
    public const byte BoxVertHorizDbl = 216;
    public const byte BoxUpLeft = 217;
    public const byte BoxDownRight = 218;
    public const byte BoxFull = 219;
    public const byte BoxHalfLower = 220;
    public const byte BoxHalfLeft = 221;
    public const byte BoxHalfRight = 222;
    public const byte BoxHalfUp = 223;
    public const byte AlphaGreek = 224;
    public const byte SLatinSharp = 225;
    public const byte GammaGreek = 226;
    public const byte PiGreek = 227;
    public const byte SigmaGreekCapital = 228;
    public const byte SigmaGreek = 229;
    public const byte Micro = 230;
    public const byte TauGreek = 231;
    public const byte PhiGreekCapital = 232;
    public const byte ThetaGreek = 233;
    public const byte OmegaGreek = 234;
    public const byte DeltaGreek = 235;
    public const byte Infinity = 236;
    public const byte PhiGreek = 237;
    public const byte EpsilonGreek = 238;
    public const byte Intersection = 239;
    public const byte Identical = 240;
    public const byte PlusMinus = 241;
    public const byte GreaterEqual = 242;
    public const byte LessEqual = 243;
    public const byte IntegralTop = 244;
    public const byte IntegralBottom = 245;
    public const byte Division = 246;
    public const byte ApproxEqual = 247;
    public const byte Degree = 248;
    public const byte Bullet = 249;
    public const byte Dot = 250;
    public const byte SquareRoot = 251;
    public const byte nLatinSuper = 252;
    public const byte TwoSuper = 253;
    public const byte Square = 254;
    public const byte NBSP = 255;
    
  }
}
