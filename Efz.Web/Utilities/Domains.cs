﻿/*
 * User: Joshua
 * Date: 23/10/2016
 * Time: 1:06 AM
 */
using System;
using System.Collections.Generic;

using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// Contains a collection of top level domain extensions and methods to aid in their
  /// use.
  /// </summary>
  public static class Domains {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of top domain extensions.
    /// </summary>
    public static string[] Collection {
      get {
        return _domains;
      }
    }
    
    /// <summary>
    /// Tree search populated with domain extensions.
    /// </summary>
    public static TreeSearch<char, string> DomainSearch {
      get {
        return _domainsSearch;
      }
    }
    
    //-------------------------------------------//
    
    private static HashSet<string> _domainsSet;
    private static TreeSearch<char, string> _domainsSearch;
    
    //-------------------------------------------//
    
    static Domains() {
      // setup the domains search
      _domainsSearch = new TreeSearch<char, string>();
      foreach(string domain in _domains) {
        string str = Chars.Stop + domain;
        DomainSearch.Add(str, str.ToCharArray());
      }
    }
    
    /// <summary>
    /// Is the specified string a domains extension.
    /// </summary>
    public static bool Contains(string extension) {
      if(_domainsSet == null) {
        _domainsSet = new HashSet<string>();
        foreach(string domain in _domains) {
          _domainsSet.Add(domain);
        }
      }
      return _domainsSet.Contains(extension.ToUppercase());
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of recognized top level domain extensions.
    /// </summary>
    private static string[] _domains = {
      "AAA",
      "AARP",
      "ABARTH",
      "ABB",
      "ABBOTT",
      "ABBVIE",
      "ABC",
      "ABLE",
      "ABOGADO",
      "ABUDHABI",
      "AC",
      "ACADEMY",
      "ACCENTURE",
      "ACCOUNTANT",
      "ACCOUNTANTS",
      "ACO",
      "ACTIVE",
      "ACTOR",
      "AD",
      "ADAC",
      "ADS",
      "ADULT",
      "AE",
      "AEG",
      "AERO",
      "AETNA",
      "AF",
      "AFAMILYCOMPANY",
      "AFL",
      "AG",
      "AGAKHAN",
      "AGENCY",
      "AI",
      "AIG",
      "AIGO",
      "AIRBUS",
      "AIRFORCE",
      "AIRTEL",
      "AKDN",
      "AL",
      "ALFAROMEO",
      "ALIBABA",
      "ALIPAY",
      "ALLFINANZ",
      "ALLSTATE",
      "ALLY",
      "ALSACE",
      "ALSTOM",
      "AM",
      "AMERICANEXPRESS",
      "AMERICANFAMILY",
      "AMEX",
      "AMFAM",
      "AMICA",
      "AMSTERDAM",
      "ANALYTICS",
      "ANDROID",
      "ANQUAN",
      "ANZ",
      "AO",
      "APARTMENTS",
      "APP",
      "APPLE",
      "AQ",
      "AQUARELLE",
      "AR",
      "ARAMCO",
      "ARCHI",
      "ARMY",
      "ARPA",
      "ART",
      "ARTE",
      "AS",
      "ASDA",
      "ASIA",
      "ASSOCIATES",
      "AT",
      "ATHLETA",
      "ATTORNEY",
      "AU",
      "AUCTION",
      "AUDI",
      "AUDIBLE",
      "AUDIO",
      "AUSPOST",
      "AUTHOR",
      "AUTO",
      "AUTOS",
      "AVIANCA",
      "AW",
      "AWS",
      "AX",
      "AXA",
      "AZ",
      "AZURE",
      "BA",
      "BABY",
      "BAIDU",
      "BANAMEX",
      "BANANAREPUBLIC",
      "BAND",
      "BANK",
      "BAR",
      "BARCELONA",
      "BARCLAYCARD",
      "BARCLAYS",
      "BAREFOOT",
      "BARGAINS",
      "BASKETBALL",
      "BAUHAUS",
      "BAYERN",
      "BB",
      "BBC",
      "BBT",
      "BBVA",
      "BCG",
      "BCN",
      "BD",
      "BE",
      "BEATS",
      "BEAUTY",
      "BEER",
      "BENTLEY",
      "BERLIN",
      "BEST",
      "BESTBUY",
      "BET",
      "BF",
      "BG",
      "BH",
      "BHARTI",
      "BI",
      "BIBLE",
      "BID",
      "BIKE",
      "BING",
      "BINGO",
      "BIO",
      "BIZ",
      "BJ",
      "BLACK",
      "BLACKFRIDAY",
      "BLANCO",
      "BLOCKBUSTER",
      "BLOG",
      "BLOOMBERG",
      "BLUE",
      "BM",
      "BMS",
      "BMW",
      "BN",
      "BNL",
      "BNPPARIBAS",
      "BO",
      "BOATS",
      "BOEHRINGER",
      "BOFA",
      "BOM",
      "BOND",
      "BOO",
      "BOOK",
      "BOOKING",
      "BOOTS",
      "BOSCH",
      "BOSTIK",
      "BOT",
      "BOUTIQUE",
      "BR",
      "BRADESCO",
      "BRIDGESTONE",
      "BROADWAY",
      "BROKER",
      "BROTHER",
      "BRUSSELS",
      "BS",
      "BT",
      "BUDAPEST",
      "BUGATTI",
      "BUILD",
      "BUILDERS",
      "BUSINESS",
      "BUY",
      "BUZZ",
      "BV",
      "BW",
      "BY",
      "BZ",
      "BZH",
      "CA",
      "CAB",
      "CAFE",
      "CAL",
      "CALL",
      "CALVINKLEIN",
      "CAM",
      "CAMERA",
      "CAMP",
      "CANCERRESEARCH",
      "CANON",
      "CAPETOWN",
      "CAPITAL",
      "CAPITALONE",
      "CAR",
      "CARAVAN",
      "CARDS",
      "CARE",
      "CAREER",
      "CAREERS",
      "CARS",
      "CARTIER",
      "CASA",
      "CASH",
      "CASINO",
      "CAT",
      "CATERING",
      "CBA",
      "CBN",
      "CBRE",
      "CBS",
      "CC",
      "CD",
      "CEB",
      "CENTER",
      "CEO",
      "CERN",
      "CF",
      "CFA",
      "CFD",
      "CG",
      "CH",
      "CHANEL",
      "CHANNEL",
      "CHASE",
      "CHAT",
      "CHEAP",
      "CHINTAI",
      "CHLOE",
      "CHRISTMAS",
      "CHROME",
      "CHRYSLER",
      "CHURCH",
      "CI",
      "CIPRIANI",
      "CIRCLE",
      "CISCO",
      "CITADEL",
      "CITI",
      "CITIC",
      "CITY",
      "CITYEATS",
      "CK",
      "CL",
      "CLAIMS",
      "CLEANING",
      "CLICK",
      "CLINIC",
      "CLINIQUE",
      "CLOTHING",
      "CLOUD",
      "CLUB",
      "CLUBMED",
      "CM",
      "CN",
      "CO",
      "COACH",
      "CODES",
      "COFFEE",
      "COLLEGE",
      "COLOGNE",
      "COM",
      "COMCAST",
      "COMMBANK",
      "COMMUNITY",
      "COMPANY",
      "COMPARE",
      "COMPUTER",
      "COMSEC",
      "CONDOS",
      "CONSTRUCTION",
      "CONSULTING",
      "CONTACT",
      "CONTRACTORS",
      "COOKING",
      "COOKINGCHANNEL",
      "COOL",
      "COOP",
      "CORSICA",
      "COUNTRY",
      "COUPON",
      "COUPONS",
      "COURSES",
      "CR",
      "CREDIT",
      "CREDITCARD",
      "CREDITUNION",
      "CRICKET",
      "CROWN",
      "CRS",
      "CRUISES",
      "CSC",
      "CU",
      "CUISINELLA",
      "CV",
      "CW",
      "CX",
      "CY",
      "CYMRU",
      "CYOU",
      "CZ",
      "DABUR",
      "DAD",
      "DANCE",
      "DATE",
      "DATING",
      "DATSUN",
      "DAY",
      "DCLK",
      "DDS",
      "DE",
      "DEAL",
      "DEALER",
      "DEALS",
      "DEGREE",
      "DELIVERY",
      "DELL",
      "DELOITTE",
      "DELTA",
      "DEMOCRAT",
      "DENTAL",
      "DENTIST",
      "DESI",
      "DESIGN",
      "DEV",
      "DHL",
      "DIAMONDS",
      "DIET",
      "DIGITAL",
      "DIRECT",
      "DIRECTORY",
      "DISCOUNT",
      "DISCOVER",
      "DISH",
      "DIY",
      "DJ",
      "DK",
      "DM",
      "DNP",
      "DO",
      "DOCS",
      "DOCTOR",
      "DODGE",
      "DOG",
      "DOHA",
      "DOMAINS",
      "DOT",
      "DOWNLOAD",
      "DRIVE",
      "DTV",
      "DUBAI",
      "DUCK",
      "DUNLOP",
      "DUNS",
      "DUPONT",
      "DURBAN",
      "DVAG",
      "DVR",
      "DZ",
      "EARTH",
      "EAT",
      "EC",
      "ECO",
      "EDEKA",
      "EDU",
      "EDUCATION",
      "EE",
      "EG",
      "EMAIL",
      "EMERCK",
      "ENERGY",
      "ENGINEER",
      "ENGINEERING",
      "ENTERPRISES",
      "EPOST",
      "EPSON",
      "EQUIPMENT",
      "ER",
      "ERICSSON",
      "ERNI",
      "ES",
      "ESQ",
      "ESTATE",
      "ESURANCE",
      "ET",
      "EU",
      "EUROVISION",
      "EUS",
      "EVENTS",
      "EVERBANK",
      "EXCHANGE",
      "EXPERT",
      "EXPOSED",
      "EXPRESS",
      "EXTRASPACE",
      "FAGE",
      "FAIL",
      "FAIRWINDS",
      "FAITH",
      "FAMILY",
      "FAN",
      "FANS",
      "FARM",
      "FARMERS",
      "FASHION",
      "FAST",
      "FEDEX",
      "FEEDBACK",
      "FERRARI",
      "FERRERO",
      "FI",
      "FIAT",
      "FIDELITY",
      "FIDO",
      "FILM",
      "FINAL",
      "FINANCE",
      "FINANCIAL",
      "FIRE",
      "FIRESTONE",
      "FIRMDALE",
      "FISH",
      "FISHING",
      "FIT",
      "FITNESS",
      "FJ",
      "FK",
      "FLICKR",
      "FLIGHTS",
      "FLIR",
      "FLORIST",
      "FLOWERS",
      "FLY",
      "FM",
      "FO",
      "FOO",
      "FOODNETWORK",
      "FOOTBALL",
      "FORD",
      "FOREX",
      "FORSALE",
      "FORUM",
      "FOUNDATION",
      "FOX",
      "FR",
      "FRESENIUS",
      "FRL",
      "FROGANS",
      "FRONTDOOR",
      "FRONTIER",
      "FTR",
      "FUJITSU",
      "FUJIXEROX",
      "FUND",
      "FURNITURE",
      "FUTBOL",
      "FYI",
      "GA",
      "GAL",
      "GALLERY",
      "GALLO",
      "GALLUP",
      "GAME",
      "GAMES",
      "GAP",
      "GARDEN",
      "GB",
      "GBIZ",
      "GD",
      "GDN",
      "GE",
      "GEA",
      "GENT",
      "GENTING",
      "GEORGE",
      "GF",
      "GG",
      "GGEE",
      "GH",
      "GI",
      "GIFT",
      "GIFTS",
      "GIVES",
      "GIVING",
      "GL",
      "GLADE",
      "GLASS",
      "GLE",
      "GLOBAL",
      "GLOBO",
      "GM",
      "GMAIL",
      "GMBH",
      "GMO",
      "GMX",
      "GN",
      "GODADDY",
      "GOLD",
      "GOLDPOINT",
      "GOLF",
      "GOO",
      "GOODHANDS",
      "GOODYEAR",
      "GOOG",
      "GOOGLE",
      "GOP",
      "GOT",
      "GOV",
      "GP",
      "GQ",
      "GR",
      "GRAINGER",
      "GRAPHICS",
      "GRATIS",
      "GREEN",
      "GRIPE",
      "GROUP",
      "GS",
      "GT",
      "GU",
      "GUARDIAN",
      "GUCCI",
      "GUGE",
      "GUIDE",
      "GUITARS",
      "GURU",
      "GW",
      "GY",
      "HAMBURG",
      "HANGOUT",
      "HAUS",
      "HBO",
      "HDFC",
      "HDFCBANK",
      "HEALTH",
      "HEALTHCARE",
      "HELP",
      "HELSINKI",
      "HERE",
      "HERMES",
      "HGTV",
      "HIPHOP",
      "HISAMITSU",
      "HITACHI",
      "HIV",
      "HK",
      "HKT",
      "HM",
      "HN",
      "HOCKEY",
      "HOLDINGS",
      "HOLIDAY",
      "HOMEDEPOT",
      "HOMEGOODS",
      "HOMES",
      "HOMESENSE",
      "HONDA",
      "HONEYWELL",
      "HORSE",
      "HOST",
      "HOSTING",
      "HOT",
      "HOTELES",
      "HOTMAIL",
      "HOUSE",
      "HOW",
      "HR",
      "HSBC",
      "HT",
      "HTC",
      "HU",
      "HUGHES",
      "HYATT",
      "HYUNDAI",
      "IBM",
      "ICBC",
      "ICE",
      "ICU",
      "ID",
      "IE",
      "IEEE",
      "IFM",
      "IINET",
      "IKANO",
      "IL",
      "IM",
      "IMAMAT",
      "IMDB",
      "IMMO",
      "IMMOBILIEN",
      "IN",
      "INDUSTRIES",
      "INFINITI",
      "INFO",
      "ING",
      "INK",
      "INSTITUTE",
      "INSURANCE",
      "INSURE",
      "INT",
      "INTEL",
      "INTERNATIONAL",
      "INTUIT",
      "INVESTMENTS",
      "IO",
      "IPIRANGA",
      "IQ",
      "IR",
      "IRISH",
      "IS",
      "ISELECT",
      "ISMAILI",
      "IST",
      "ISTANBUL",
      "IT",
      "ITAU",
      "ITV",
      "IWC",
      "JAGUAR",
      "JAVA",
      "JCB",
      "JCP",
      "JE",
      "JEEP",
      "JETZT",
      "JEWELRY",
      "JLC",
      "JLL",
      "JM",
      "JMP",
      "JNJ",
      "JO",
      "JOBS",
      "JOBURG",
      "JOT",
      "JOY",
      "JP",
      "JPMORGAN",
      "JPRS",
      "JUEGOS",
      "JUNIPER",
      "KAUFEN",
      "KDDI",
      "KE",
      "KERRYHOTELS",
      "KERRYLOGISTICS",
      "KERRYPROPERTIES",
      "KFH",
      "KG",
      "KH",
      "KI",
      "KIA",
      "KIM",
      "KINDER",
      "KINDLE",
      "KITCHEN",
      "KIWI",
      "KM",
      "KN",
      "KOELN",
      "KOMATSU",
      "KOSHER",
      "KP",
      "KPMG",
      "KPN",
      "KR",
      "KRD",
      "KRED",
      "KUOKGROUP",
      "KW",
      "KY",
      "KYOTO",
      "KZ",
      "LA",
      "LACAIXA",
      "LADBROKES",
      "LAMBORGHINI",
      "LAMER",
      "LANCASTER",
      "LANCIA",
      "LANCOME",
      "LAND",
      "LANDROVER",
      "LANXESS",
      "LASALLE",
      "LAT",
      "LATINO",
      "LATROBE",
      "LAW",
      "LAWYER",
      "LB",
      "LC",
      "LDS",
      "LEASE",
      "LECLERC",
      "LEFRAK",
      "LEGAL",
      "LEGO",
      "LEXUS",
      "LGBT",
      "LI",
      "LIAISON",
      "LIDL",
      "LIFE",
      "LIFEINSURANCE",
      "LIFESTYLE",
      "LIGHTING",
      "LIKE",
      "LILLY",
      "LIMITED",
      "LIMO",
      "LINCOLN",
      "LINDE",
      "LINK",
      "LIPSY",
      "LIVE",
      "LIVING",
      "LIXIL",
      "LK",
      "LOAN",
      "LOANS",
      "LOCKER",
      "LOCUS",
      "LOFT",
      "LOL",
      "LONDON",
      "LOTTE",
      "LOTTO",
      "LOVE",
      "LPL",
      "LPLFINANCIAL",
      "LR",
      "LS",
      "LT",
      "LTD",
      "LTDA",
      "LU",
      "LUNDBECK",
      "LUPIN",
      "LUXE",
      "LUXURY",
      "LV",
      "LY",
      "MA",
      "MACYS",
      "MADRID",
      "MAIF",
      "MAISON",
      "MAKEUP",
      "MAN",
      "MANAGEMENT",
      "MANGO",
      "MARKET",
      "MARKETING",
      "MARKETS",
      "MARRIOTT",
      "MARSHALLS",
      "MASERATI",
      "MATTEL",
      "MBA",
      "MC",
      "MCD",
      "MCDONALDS",
      "MCKINSEY",
      "MD",
      "ME",
      "MED",
      "MEDIA",
      "MEET",
      "MELBOURNE",
      "MEME",
      "MEMORIAL",
      "MEN",
      "MENU",
      "MEO",
      "METLIFE",
      "MG",
      "MH",
      "MIAMI",
      "MICROSOFT",
      "MIL",
      "MINI",
      "MINT",
      "MIT",
      "MITSUBISHI",
      "MK",
      "ML",
      "MLB",
      "MLS",
      "MM",
      "MMA",
      "MN",
      "MO",
      "MOBI",
      "MOBILY",
      "MODA",
      "MOE",
      "MOI",
      "MOM",
      "MONASH",
      "MONEY",
      "MONSTER",
      "MONTBLANC",
      "MOPAR",
      "MORMON",
      "MORTGAGE",
      "MOSCOW",
      "MOTORCYCLES",
      "MOV",
      "MOVIE",
      "MOVISTAR",
      "MP",
      "MQ",
      "MR",
      "MS",
      "MSD",
      "MT",
      "MTN",
      "MTPC",
      "MTR",
      "MU",
      "MUSEUM",
      "MUTUAL",
      "MUTUELLE",
      "MV",
      "MW",
      "MX",
      "MY",
      "MZ",
      "NA",
      "NAB",
      "NADEX",
      "NAGOYA",
      "NAME",
      "NATIONWIDE",
      "NATURA",
      "NAVY",
      "NBA",
      "NC",
      "NE",
      "NEC",
      "NET",
      "NETBANK",
      "NETFLIX",
      "NETWORK",
      "NEUSTAR",
      "NEW",
      "NEWS",
      "NEXT",
      "NEXTDIRECT",
      "NEXUS",
      "NF",
      "NFL",
      "NG",
      "NGO",
      "NHK",
      "NI",
      "NICO",
      "NIKE",
      "NIKON",
      "NINJA",
      "NISSAN",
      "NISSAY",
      "NL",
      "NO",
      "NOKIA",
      "NORTHWESTERNMUTUAL",
      "NORTON",
      "NOW",
      "NOWRUZ",
      "NOWTV",
      "NP",
      "NR",
      "NRA",
      "NRW",
      "NTT",
      "NU",
      "NYC",
      "NZ",
      "OBI",
      "OBSERVER",
      "OFF",
      "OFFICE",
      "OKINAWA",
      "OLAYAN",
      "OLAYANGROUP",
      "OLDNAVY",
      "OLLO",
      "OM",
      "OMEGA",
      "ONE",
      "ONG",
      "ONL",
      "ONLINE",
      "ONYOURSIDE",
      "OOO",
      "OPEN",
      "ORACLE",
      "ORANGE",
      "ORG",
      "ORGANIC",
      "ORIENTEXPRESS",
      "ORIGINS",
      "OSAKA",
      "OTSUKA",
      "OTT",
      "OVH",
      "PA",
      "PAGE",
      "PAMPEREDCHEF",
      "PANASONIC",
      "PANERAI",
      "PARIS",
      "PARS",
      "PARTNERS",
      "PARTS",
      "PARTY",
      "PASSAGENS",
      "PAY",
      "PCCW",
      "PE",
      "PET",
      "PF",
      "PFIZER",
      "PG",
      "PH",
      "PHARMACY",
      "PHILIPS",
      "PHOTO",
      "PHOTOGRAPHY",
      "PHOTOS",
      "PHYSIO",
      "PIAGET",
      "PICS",
      "PICTET",
      "PICTURES",
      "PID",
      "PIN",
      "PING",
      "PINK",
      "PIONEER",
      "PIZZA",
      "PK",
      "PL",
      "PLACE",
      "PLAY",
      "PLAYSTATION",
      "PLUMBING",
      "PLUS",
      "PM",
      "PN",
      "PNC",
      "POHL",
      "POKER",
      "POLITIE",
      "PORN",
      "POST",
      "PR",
      "PRAMERICA",
      "PRAXI",
      "PRESS",
      "PRIME",
      "PRO",
      "PROD",
      "PRODUCTIONS",
      "PROF",
      "PROGRESSIVE",
      "PROMO",
      "PROPERTIES",
      "PROPERTY",
      "PROTECTION",
      "PRU",
      "PRUDENTIAL",
      "PS",
      "PT",
      "PUB",
      "PW",
      "PWC",
      "PY",
      "QA",
      "QPON",
      "QUEBEC",
      "QUEST",
      "QVC",
      "RACING",
      "RADIO",
      "RAID",
      "RE",
      "READ",
      "REALESTATE",
      "REALTOR",
      "REALTY",
      "RECIPES",
      "RED",
      "REDSTONE",
      "REDUMBRELLA",
      "REHAB",
      "REISE",
      "REISEN",
      "REIT",
      "REN",
      "RENT",
      "RENTALS",
      "REPAIR",
      "REPORT",
      "REPUBLICAN",
      "REST",
      "RESTAURANT",
      "REVIEW",
      "REVIEWS",
      "REXROTH",
      "RICH",
      "RICHARDLI",
      "RICOH",
      "RIGHTATHOME",
      "RIO",
      "RIP",
      "RO",
      "ROCHER",
      "ROCKS",
      "RODEO",
      "ROGERS",
      "ROOM",
      "RS",
      "RSVP",
      "RU",
      "RUHR",
      "RUN",
      "RW",
      "RWE",
      "RYUKYU",
      "SA",
      "SAARLAND",
      "SAFE",
      "SAFETY",
      "SAKURA",
      "SALE",
      "SALON",
      "SAMSCLUB",
      "SAMSUNG",
      "SANDVIK",
      "SANDVIKCOROMANT",
      "SANOFI",
      "SAP",
      "SAPO",
      "SARL",
      "SAS",
      "SAVE",
      "SAXO",
      "SB",
      "SBI",
      "SBS",
      "SC",
      "SCA",
      "SCB",
      "SCHAEFFLER",
      "SCHMIDT",
      "SCHOLARSHIPS",
      "SCHOOL",
      "SCHULE",
      "SCHWARZ",
      "SCIENCE",
      "SCJOHNSON",
      "SCOR",
      "SCOT",
      "SD",
      "SE",
      "SEAT",
      "SECURE",
      "SECURITY",
      "SEEK",
      "SELECT",
      "SENER",
      "SERVICES",
      "SES",
      "SEVEN",
      "SEW",
      "SEX",
      "SEXY",
      "SFR",
      "SG",
      "SH",
      "SHANGRILA",
      "SHARP",
      "SHAW",
      "SHELL",
      "SHIA",
      "SHIKSHA",
      "SHOES",
      "SHOP",
      "SHOPPING",
      "SHOUJI",
      "SHOW",
      "SHOWTIME",
      "SHRIRAM",
      "SI",
      "SILK",
      "SINA",
      "SINGLES",
      "SITE",
      "SJ",
      "SK",
      "SKI",
      "SKIN",
      "SKY",
      "SKYPE",
      "SL",
      "SLING",
      "SM",
      "SMART",
      "SMILE",
      "SN",
      "SNCF",
      "SO",
      "SOCCER",
      "SOCIAL",
      "SOFTBANK",
      "SOFTWARE",
      "SOHU",
      "SOLAR",
      "SOLUTIONS",
      "SONG",
      "SONY",
      "SOY",
      "SPACE",
      "SPIEGEL",
      "SPOT",
      "SPREADBETTING",
      "SR",
      "SRL",
      "SRT",
      "ST",
      "STADA",
      "STAPLES",
      "STAR",
      "STARHUB",
      "STATEBANK",
      "STATEFARM",
      "STATOIL",
      "STC",
      "STCGROUP",
      "STOCKHOLM",
      "STORAGE",
      "STORE",
      "STREAM",
      "STUDIO",
      "STUDY",
      "STYLE",
      "SU",
      "SUCKS",
      "SUPPLIES",
      "SUPPLY",
      "SUPPORT",
      "SURF",
      "SURGERY",
      "SUZUKI",
      "SV",
      "SWATCH",
      "SWIFTCOVER",
      "SWISS",
      "SX",
      "SY",
      "SYDNEY",
      "SYMANTEC",
      "SYSTEMS",
      "SZ",
      "TAB",
      "TAIPEI",
      "TALK",
      "TAOBAO",
      "TARGET",
      "TATAMOTORS",
      "TATAR",
      "TATTOO",
      "TAX",
      "TAXI",
      "TC",
      "TCI",
      "TD",
      "TDK",
      "TEAM",
      "TECH",
      "TECHNOLOGY",
      "TEL",
      "TELECITY",
      "TELEFONICA",
      "TEMASEK",
      "TENNIS",
      "TEVA",
      "TF",
      "TG",
      "TH",
      "THD",
      "THEATER",
      "THEATRE",
      "TIAA",
      "TICKETS",
      "TIENDA",
      "TIFFANY",
      "TIPS",
      "TIRES",
      "TIROL",
      "TJ",
      "TJMAXX",
      "TJX",
      "TK",
      "TKMAXX",
      "TL",
      "TM",
      "TMALL",
      "TN",
      "TO",
      "TODAY",
      "TOKYO",
      "TOOLS",
      "TOP",
      "TORAY",
      "TOSHIBA",
      "TOTAL",
      "TOURS",
      "TOWN",
      "TOYOTA",
      "TOYS",
      "TR",
      "TRADE",
      "TRADING",
      "TRAINING",
      "TRAVEL",
      "TRAVELCHANNEL",
      "TRAVELERS",
      "TRAVELERSINSURANCE",
      "TRUST",
      "TRV",
      "TT",
      "TUBE",
      "TUI",
      "TUNES",
      "TUSHU",
      "TV",
      "TVS",
      "TW",
      "TZ",
      "UA",
      "UBANK",
      "UBS",
      "UCONNECT",
      "UG",
      "UK",
      "UNICOM",
      "UNIVERSITY",
      "UNO",
      "UOL",
      "UPS",
      "US",
      "UY",
      "UZ",
      "VA",
      "VACATIONS",
      "VANA",
      "VANGUARD",
      "VC",
      "VE",
      "VEGAS",
      "VENTURES",
      "VERISIGN",
      "VERSICHERUNG",
      "VET",
      "VG",
      "VI",
      "VIAJES",
      "VIDEO",
      "VIG",
      "VIKING",
      "VILLAS",
      "VIN",
      "VIP",
      "VIRGIN",
      "VISA",
      "VISION",
      "VISTA",
      "VISTAPRINT",
      "VIVA",
      "VIVO",
      "VLAANDEREN",
      "VN",
      "VODKA",
      "VOLKSWAGEN",
      "VOTE",
      "VOTING",
      "VOTO",
      "VOYAGE",
      "VU",
      "VUELOS",
      "WALES",
      "WALMART",
      "WALTER",
      "WANG",
      "WANGGOU",
      "WARMAN",
      "WATCH",
      "WATCHES",
      "WEATHER",
      "WEATHERCHANNEL",
      "WEBCAM",
      "WEBER",
      "WEBSITE",
      "WED",
      "WEDDING",
      "WEIBO",
      "WEIR",
      "WF",
      "WHOSWHO",
      "WIEN",
      "WIKI",
      "WILLIAMHILL",
      "WIN",
      "WINDOWS",
      "WINE",
      "WINNERS",
      "WME",
      "WOLTERSKLUWER",
      "WOODSIDE",
      "WORK",
      "WORKS",
      "WORLD",
      "WOW",
      "WS",
      "WTC",
      "WTF",
      "XBOX",
      "XEROX",
      "XFINITY",
      "XIHUAN",
      "XIN",
      "XPERIA",
      "XXX",
      "XYZ",
      "YACHTS",
      "YAHOO",
      "YAMAXUN",
      "YANDEX",
      "YE",
      "YODOBASHI",
      "YOGA",
      "YOKOHAMA",
      "YOU",
      "YOUTUBE",
      "YT",
      "YUN",
      "ZA",
      "ZAPPOS",
      "ZARA",
      "ZERO",
      "ZIP",
      "ZIPPO",
      "ZM",
      "ZONE",
      "ZUERICH",
      "ZW",
    };
    
    //-------------------------------------------//
    
  }
  
}
