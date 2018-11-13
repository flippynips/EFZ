/*
 * User: Joshua
 * Date: 25/06/2016
 * Time: 3:16 PM
 */

namespace Efz {
  
  /// <summary>
  /// Description of Ascii chars.
  /// </summary>
  public static class Chars {
    
    /// <summary>
    /// Whitespace characters.
    /// </summary>
    public static readonly char[] Whitespace = { Chars.Tab, Chars.NewLine, Chars.CarriageReturn, Chars.Space };
    /// <summary>
    /// Characters allowed in a url without requiring to be encoded.
    /// </summary>
    public static readonly char[] UrlCharacters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '=', '#', ';', ':', '/', '%', '$', '-', '_', '.', '+', '!', '*', '\'', '(', ')', ',' };
    /// <summary>
    /// Characters in the alphabet.
    /// </summary>
    public static readonly char[] Alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    
    // control
    public const char Null = '\0';
    public const char StartHeading = '';
    public const char StartText = '';
    public const char EndText = '';
    public const char EndTransmission = '';
    public const char Enquiry = '';
    public const char Acknowledge = '';
    public const char Bell = '';
    public const char Backspace = '';
    public const char Tab = '\t';
    public const char NewLine = '\n';
    public const char TabVertical = '';
    public const char NewPage = '';
    public const char CarriageReturn = '\r';
    public const char ShiftOut = '';
    public const char ShiftIn = '';
    public const char DataLink = '';
    public const char Device1 = '';
    public const char Device2 = '';
    public const char Device3 = '';
    public const char Device4 = '';
    public const char AcknowledgeNegative = '';
    public const char Idle = '';
    public const char EndBlock = '';
    public const char Cancel = '';
    public const char EndMedium = '';
    public const char Substitute = '';
    public const char Escape = '';
    public const char SeparatorFile = '';
    public const char SeparatorGroup = '';
    public const char SeparatorRecord = '';
    public const char SeparatorUnit = '';
    
    // punctuation and symbols
    public const char Space = ' ';
    public const char Exclamation = '!';
    public const char DoubleQuote = '"';
    public const char Hash = '#';
    public const char Dollar = '$';
    public const char Percent = '%';
    public const char And = '&';
    public const char Quote = '\'';
    public const char BracketOpen = '(';
    public const char BracketClose = ')';
    public const char Star = '*';
    public const char Plus = '+';
    public const char Comma = ',';
    public const char Dash = '-';
    public const char Stop = '.';
    public const char ForwardSlash = '/';
    
    public const char Colon = ':';
    public const char SemiColon = ';';
    public const char LessThan = '<';
    public const char Equal = '=';
    public const char GreaterThan = '>';
    public const char Question = '?';
    public const char At = '@';
    
    public const char BracketSqOpen = '[';
    public const char BackSlash = '\\';
    public const char BracketSqClose = ']';
    public const char Caret = '^';
    public const char Underscore = '_';
    public const char Accent = '`';
    
    public const char BraceOpen = '{';
    public const char Bar = '|';
    public const char BraceClose = '}';
    public const char Tilde = '~';
    public const char Delete = '';
    
    // numbers
    public const char n0 = '0';
    public const char n1 = '1';
    public const char n2 = '2';
    public const char n3 = '3';
    public const char n4 = '4';
    public const char n5 = '5';
    public const char n6 = '6';
    public const char n7 = '7';
    public const char n8 = '8';
    public const char n9 = '9';
    
    // letters
    public const char a = 'a';
    public const char b = 'b';
    public const char c = 'c';
    public const char d = 'd';
    public const char e = 'e';
    public const char f = 'f';
    public const char g = 'g';
    public const char h = 'h';
    public const char i = 'i';
    public const char j = 'j';
    public const char k = 'k';
    public const char l = 'l';
    public const char m = 'm';
    public const char n = 'n';
    public const char o = 'o';
    public const char p = 'p';
    public const char q = 'q';
    public const char r = 'r';
    public const char s = 's';
    public const char t = 't';
    public const char u = 'u';
    public const char v = 'v';
    public const char w = 'w';
    public const char x = 'x';
    public const char y = 'y';
    public const char z = 'z';
    
    public const char A = 'A';
    public const char B = 'B';
    public const char C = 'C';
    public const char D = 'D';
    public const char E = 'E';
    public const char F = 'F';
    public const char G = 'G';
    public const char H = 'H';
    public const char I = 'I';
    public const char J = 'J';
    public const char K = 'K';
    public const char L = 'L';
    public const char M = 'M';
    public const char N = 'N';
    public const char O = 'O';
    public const char P = 'P';
    public const char Q = 'Q';
    public const char R = 'R';
    public const char S = 'S';
    public const char T = 'T';
    public const char U = 'U';
    public const char V = 'V';
    public const char W = 'W';
    public const char X = 'X';
    public const char Y = 'Y';
    public const char Z = 'Z';
    
    // extended
    public const char CLatinCedil = 'Ç';
    public const char uLatinDieres = 'ü';
    public const char eLatinAcute = 'é';
    public const char aLatinCircum = 'â';
    public const char aLatinDieres = 'ä';
    public const char aLatinGrave = 'à';
    public const char aLatinRing = 'å';
    public const char cLatinCedil = 'ç';
    public const char eLatinCircum = 'ê';
    public const char eLatinDieres = 'ë';
    public const char eLatinGrave = 'è';
    public const char iLatinDieres = 'ï';
    public const char iLatinCircum = 'î';
    public const char iLatinGrave = 'ì';
    public const char ALatinDieres = 'Ä';
    public const char ALatinRing = 'Å';
    public const char ELatinAcute = 'É';
    public const char aeLatinLiga = 'æ';
    public const char AELatinLiga = 'Æ';
    public const char oLatinCircum = 'ô';
    public const char oLatinDieres = 'ö';
    public const char oLatinGrave = 'ò';
    public const char uLatinCircum = 'û';
    public const char uLatinGrave = 'ù';
    public const char yLatinDieres = 'ÿ';
    public const char OLatinDieres = 'Ö';
    public const char ULatinDieres = 'Ü';
    public const char Cent = 'ø';
    public const char Asterisk = '£';
    public const char Yen = 'Ø';
    public const char Peseta = '×';
    public const char fLatinHook = 'ƒ';
    public const char aLatinAcute = 'á';
    public const char iLatinAcute = 'í';
    public const char oLatinAcute = 'ó';
    public const char uLatinAcute = 'ú';
    public const char nLatinTilde = 'ñ';
    public const char NLatinTilde = 'Ñ';
    public const char Feminine = 'ª';
    public const char Masculine = 'º';
    public const char QuestionInvert = '¿';
    public const char ReversedNot = '®';
    public const char Not = '¬';
    public const char Half = '½';
    public const char Quater = '¼';
    public const char ExclaimInvert = '¡';
    public const char DoubleLeft = '«';
    public const char DoubleRight = '»';
    public const char BoxFullLight = '░';
    public const char BoxFullMedium = '▒';
    public const char BoxFullDark = '▓';
    public const char BoxVert = '│';
    public const char BoxVertLeft = '┤';
    public const char BoxVertLeftDbl = '╡';
    public const char BoxVertDblLeft = '╢';
    public const char BoxDownDblLeft = '╖';
    public const char BoxDownLeftDbl = '╕';
    public const char BoxVertDblLeftDbl = '╣';
    public const char BoxVertDbl = '║';
    public const char BoxDownDblLeftDbl = '╗';
    public const char BoxUpDblLeftDbl = '╝';
    public const char BoxUpDblLeft = '╜';
    public const char BoxUpLeftDbl = '╛';
    public const char BoxDownLeft = '┐';
    public const char BoxUpRight = '└';
    public const char BoxUpHoriz = '┴';
    public const char BoxDownHoriz = '┬';
    public const char BoxVertRight = '├';
    public const char BoxHoriz = '─';
    public const char BoxVertHoriz = '┼';
    public const char BoxVertRightDbl = '╞';
    public const char BoxVertDblRight = '╟';
    public const char BoxUpDblRightDbl = '╚';
    public const char BoxDownDblRightDbl = '╔';
    public const char BoxUpDblHorizDbl = '╩';
    public const char BoxDownDblHorizDbl = '╦';
    public const char BoxVertDblRightDbl = '╠';
    public const char BoxHorizDbl = '═';
    public const char BoxVertDblHorizDbl = '╬';
    public const char BoxUpHorizDbl = '╧';
    public const char BoxUpDblHoriz = '╨';
    public const char BoxDownHorizDbl = '╤';
    public const char BoxDownDblHoriz = '╥';
    public const char BoxUpDblRight = '╙';
    public const char BoxUpRightDbl = '╘';
    public const char BoxDownRightDbl = '╒';
    public const char BoxDownDblRight = '╓';
    public const char BoxVertDblHoriz = '╫';
    public const char BoxVertHorizDbl = '╪';
    public const char BoxUpLeft = '┘';
    public const char BoxDownRight = '┌';
    public const char BoxFull = '█';
    public const char BoxHalfLower = '▄';
    public const char BoxHalfLeft = '▌';
    public const char BoxHalfRight = '▐';
    public const char BoxHalfUp = '▀';
    public const char AlphaGreek = 'α';
    public const char SLatinSharp = 'ß';
    public const char GammaGreek = 'Γ';
    public const char PiGreek = 'π';
    public const char SigmaGreekCapital = 'Σ';
    public const char SigmaGreek = 'σ';
    public const char Micro = 'µ';
    public const char TauGreek = 'τ';
    public const char PhiGreekCapital = 'Φ';
    public const char ThetaGreek = 'Θ';
    public const char OmegaGreek = 'Ω';
    public const char DeltaGreek = 'δ';
    public const char Infinity = '∞';
    public const char PhiGreek = 'φ';
    public const char EpsilonGreek = 'ε';
    public const char Intersection = '∩';
    public const char Identical = '≡';
    public const char PlusMinus = '±';
    public const char GreaterEqual = '≥';
    public const char LessEqual = '≤';
    public const char IntegralTop = '⌠';
    public const char IntegralBottom = '⌡';
    public const char Division = '÷';
    public const char ApproxEqual = '≈';
    public const char Degree = '°';
    public const char Bullet = '∙';
    public const char Dot = '•';
    public const char SquareRoot = '√';
    public const char nLatinSuper = 'ⁿ';
    public const char TwoSuper = '²';
    public const char Square = '■';
    public const char NBSP = ' ';
    
  }
  
}
