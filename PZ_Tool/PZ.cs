using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PZ
{
    public class FT4
    {
        public FT4() { VERTICES = new int[0xC]; UVS = new byte[0x8]; }

        public int[] VERTICES;
        public byte[] UVS;
        public short CPAGE;
        public short TPAGE;
    }

    public class VECTOR3
    {
        public VECTOR3() { }

        public int X;
        public int Y;
        public int Z;
    }

    public class OBJECT
    {
        public OBJECT() { POLYGONS = new List<FT4>(); }

        public List<FT4> POLYGONS;
    }

    public static readonly int TRANSLATION_FACTOR = 10000;

    public static readonly int TANK_SZ = 0xFEA4;
    public static readonly int TANK_MODEL_SZ = 0x4060;
    public static readonly int TANK_TEXTURE_SZ = 0x8000;
    public static readonly int TANK_PALETTE_SZ = 0x80;
    public static readonly int TANK_POSITION_SZ = 0x120;
    public static readonly int TANK_SKIP_SZ = 0x145F;
    public static readonly int TANK_TIM_SZ = 0x1420;

    public static readonly int TEXTURE_W_MAX = 256;
    public static readonly int TEXTURE_H_MAX = 256;
    public static readonly int TANK_TEXTURE_W = 128;
    public static readonly int TANK_TEXTURE_H = TEXTURE_H_MAX;
    public static readonly int TANK_PAL_SZ = 64;

    public static readonly int MINIMAP_SZ = 0x2040;

    public static readonly string[] TANK_NAME_TABLE = {
        "ＰｚⅢＪ／Ｌ６０", //0x00
        "ＳｔｕＧⅢ　Ｇ", //0x01
        "ＰｚⅣ　Ｈ　ｓ", //0x02
        "ＰｚⅣ／７０（Ｖ", //0x03
        "Ｐａｎｔｈｅｒ　Ａ", //0x04
        "Ｐａｎｔｈｅｒ　Ｇ", //0x05
        "Ｔｉｇｅｒ　Ⅰ　Ｅ", //0x06
        "Ｔｉｇｅｒ　Ⅱ", //0x07
        "ＪａｇｄＴｉｇｅｒ", //0x08
        "Ｎａｓｈｏｒｎ", //0x09
        "Ｈｅｔｚｅｒ", //0x0A
        "７．５ｃｍ　Ｐａｋ４０", //0x0B
        "８．８ｃｍ　Ｆｌａｋ", //0x0C
        "Ｔｉｇｅｒ　Ⅰ", //0x0D
        "Ｗｉｒｂｅｌｗｉｎｄ", //0x0E
        "Ｎｅｂｅｌｗｅｒｆｅｒ", //0x0F
        "Ｓｄｋｆｚ２５１／Ｃ", //0x10
        "Ｏｐｅｌ", //0x11
        "ＫｕｂｅｌＷａｇｅｎ", //0x12
        "５ｃｍ　Ｐａｋ３８", //0x13
        "Ｈｏｒｎｉｓｓｅ", //0x14
        "ＰｚⅢ　Ｎ", //0x15
        "Ｐａｎｔｈｅｒ／Ｍ１０", //0x16
        "Ｔｉｇｅｒ　Ⅰ　Ｌ", //0x17
        "Ｍａｒｄｅｒ　Ⅱ", //0x18
        "Ｓｄｋｆｚ２５１／Ｄ", //0x19
        "ＰｚⅢＪ／Ｌ４２", //0x1A
        "ＰｚⅣ　Ｇ", //0x1B
        "ＰｚＪａｇｄⅣ／Ｌ４８", //0x1C
        "Ｓｄｋｆｚ７／１", //0x1D
        "ＰｚⅢ　Ｎ　ｓ", //0x1E
        "ＳｔｕＧⅢ　Ｅ", //0x1F
        "ＰｚⅣ　Ｈ　ｔｕｒ　ｓ", //0x20
        "Ｔｉｇｅｒ　Ⅰ　Ｌ", //0x21
        "Ｔｉｇｅｒ　Ⅱ", //0x22
        "Ｔｉｇｅｒ　Ⅰ　Ｅ", //0x23
        "Ｍａｕｓ", //0x24
        "Ｅｌｅｆａｎｔ", //0x25
        "ＪａｇｄＰａｎｔｈｅｒ", //0x26
        "ＪａｇｄＴｉｇｅｒ", //0x27
        "Ｍ３　Ｈｏｎｅｙ", //0x28
        "Ｃｒｏｍｗｅｌｌ　Ⅳ", //0x29
        "Ｆｉｒｅｆｌｙ　Ｖｃ", //0x2A
        "Ｍ５Ａ１", //0x2B
        "Ｃｈｕｒｃｈｉｌｌ　Ⅶ", //0x2C
        "Ｍ１　ＧＵＮ", //0x2D
        "Ｒ　Ｃａｒｒｉｅｒ", //0x2E
        "Ｕ　Ｃａｒｒｉｅｒ", //0x2F
        "Ｆｉｒｅｆｌｙ　Ｖｃ", //0x30
        "Ｆｉｒｅｆｌｙ　Ｖｃ", //0x31
        "Т－６０", //0x32
        "Т－３４　（１９４２）", //0x33
        "Т－３４－８５", //0x34
        "СУ－８５", //0x35
        "ИСУ－１５２", //0x36
        "ИС－２", //0x37
        "КВ－１　（１９４０）", //0x38
        "Т－３５", //0x39
        "７６．２　Ф３２", //0x3A
        "１２２ｍｍ　ＦＨ", //0x3B
        "Ｋａｔｙｕｓｈａ", //0x3C
        "Т－７０", //0x3D
        "Ｚｉｓ５", //0x3E
        "Т－３４　（１９４１）", //0x3F
        "КВ－１　（１９４２）", //0x40
        "БП４３", //0x41
        "НКЛ－２６", //0x42
        "БП４３　Ｔｅｎ", //0x43
        "БП４３　Ｌｏｃ", //0x44
        "БП４３　Ｗａｇｏｎ", //0x45
        "Т－３４－８５", //0x46
        "Т－３５", //0x47
        "ИС－２М", //0x48
        "Т－３４　（１９４１）", //0x49
        "Т－３４　（１９４２）", //0x4A
        "БП４３", //0x4B
        "ИС－３", //0x4C
        "ИС－３", //0x4D
        "ИС－３", //0x4E
        "ИС－３", //0x4F
        "Ｍ３", //0x50
        "Ｍ４", //0x51
        "Ｍ４Ａ１（７６）", //0x52
        "Ｍ４Ａ３Ｅ２", //0x53
        "Ｍ１０", //0x54
        "Ｍ１６ＡＡ", //0x55
        "１０５ｍｍ", //0x56
        "ＷＩＬＬＹＳ　ＭＢ", //0x57
        "Ｍ３Ｈａｌｆｔｒｕｃｋ", //0x58
        "Ｍ３Ｈａｌｆｔｒｕｃｋ", //0x59
        "＊＊＊＊＊＊＊＊＊", //0x5A
        "Ｔ２６Ｅ３", //0x5B
        "Ｍ４Ａ１（７６）", //0x5C
        "Ｍ４Ａ１（７６）", //0x5D
        "Ｍ４Ａ１（７６）", //0x5E
        "Ｍ４Ａ１（７６）", //0x5F
        "Ｍ４Ａ１（７６）", //0x60
        "Ｍ４Ａ１（７６）", //0x61
        "Ｍ４Ａ１（７６）", //0x62
        "Ｍ４Ａ１（７６）", //0x63
        "Ａｕｒｅｏｌｅ／８８", //0x64
        "Ａｕｒｅｏｌｅ　ｓ", //0x65
        "Ｅ－７９／１２８", //0x66
        "Ｔ６９Ｅ３", //0x67
        "ИС－１５２", //0x68
        "СУ－１２２с", //0x69
        "Ｍ１Ａ２　ＡＢＲＡＭＳ", //0x6A
        "ＳｈｏｒｔＢｕｌｌ", //0x6B
        "Ａｕｒｅｏｌｅ／７５", //0x6C
        "Ｅ－７９／８８", //0x6D
        "Ａｕｒｅｏｌｅ　ｓ／ｌ", //0x6E
        "Ａｕｒｅｏｌｅ　ｓ", //0x6F
        "Ｅ－７９／１２８", //0x70
        "Ｔ６９Ｅ３", //0x71
        "ИС－１５２", //0x72
        "СУ－１２２с", //0x73
        "ＳｈｏｒｔＢｕｌｌ", //0x74
        "Ａｕｒｅｏｌｅ／７５", //0x75
        "Ｅ－７９／８８", //0x76
        "Ａｕｒｅｏｌｅ　ｓ／ｌ", //0x77
        "Ａｕｒｅｏｌｅ／８８　", //0x78
        "Ｍ１Ａ２　ＡＢＲＡＭＳ", //0x79
        "ИС－１５２Ｍ", //0x7A
        "ИС－１５２Ｍ", //0x7B
        "Ｔ－８０УД", //0x7C
        "Ｍ１Ａ２　ＡＢＲＡＭＳ", //0x7D
        "Ｔｙｐｅ１Ｃｈｉ－Ｈｅ", //0x7E
        "Ｅ－７９　Ｂｉｓ", //0x7F
        "Ｔｙｐｅ３Ｃｈｉ－Ｎｕ", //0x80
        "Ｔｙｐｅ４Ｃｈｉ－Ｔｏ", //0x81
        "Ｊｕ８７Ｄ", //0x82
        "Ｊｕ８７Ｇ", //0x83
        "Ｈｓ１２９", //0x84
        "Ｐ４７", //0x85
        "Ил－２М", //0x86
        "ＴＹＰＨＯＯＮ", //0x87
        "Ｓ　Ｌｉｇｈｔｎｉｎｇ" //0x88
    };

    public static readonly string[] LEVEL_NAME_TABLE = {
        "Misty Forest (Kharkov)",
        "Olchowatka (Kharkov)",
        "Kharkov (Kharkov)",
        "Hill 220.5 (Kursk)",
        "Teterevino (Kursk)",
        "Hill 241.6 (Kursk)",
        "Oktjabrskij (Kursk)",
        "Meadow ／ Plain (Kursk)",
        "South ／ Mga (Leningrad)",
        "Railway ／ Kirov (Leningrad)",
        "Gaytalobo (Leningrad)",
        "Neva (Leningrad)",
        "Poselok No. 5 (Leningrad)",
        "Sinyavino (Leningrad)",
        "Junction (Leningrad)",
        "Pulkovo highland (Leningrad)",
        "Krasnoe Sero (Leningrad)",
        "Snowfield 1 (Russia)",
        "Snowfield 2 (Russia)",
        "Snowfield 3 (Russia)",
        "Vitebsk (Russia)",
        "Villers Bocage (Normandy)",
        "Odon River (Normandy)",
        "St. Jean-de-Daye (Normandy)",
        "Chateau (Normandy)",
        "Le Dezert (Normandy)",
        "Hill 112 (Normandy)",
        "Northwest St.Lo (Normandy)",
        "Le Neufbourg (Normandy)",
        "Soule River (Normandy)",
        "St. Barthelemy (Normandy)",
        "N158 Highway (Normandy)",
        "Orne River (Normandy)",
        "Argentan (Normandy)",
        "Malmedy (Ardennes)",
        "Poteau (Ardennes)",
        "Saint-Vith (Ardennes)",
        "Ambleve River (Ardennes)",
        "Rod (Ardennes)",
        "Manhay (Ardennes)",
        "Baraque de Fraiture (Ardennes)",
        "Maldinger (Ardennes)",
        "Remagen (Germany)",
        "Sachsendorf (Germany)",
        "Gorgast (Germany)",
        "Seelow (Germany)",
        "Berlin (Germany)",
        "Kushira (Japan)",
        "City of Death (Necropolis)",
        "Proving Ground (AD2002)",
    };

    public static readonly int MAP_TERRAIN_CHUNK_SZ = 0x38D08;
    public static readonly int MAP_TEXTURES_CHUNK_SZ = 0x4A200;
    public static readonly short[] MAP_TPAGE_TABLE = new short[] {0x15, 0x16, 0x17, 0x18};

    public static readonly byte[] MAP_UV_TABLE = new byte[] {
        0x00, 0x00,
        0x20, 0x00,
        0x40, 0x00,
        0x60, 0x00,
        0x80, 0x00,
        0xA0, 0x00,
        0xC0, 0x00,
        0xE0, 0x00,
        0x00, 0x20,
        0x20, 0x20,
        0x40, 0x20,
        0x60, 0x20,
        0x80, 0x20,
        0xA0, 0x20,
        0xC0, 0x20,
        0xE0, 0x20,
        0x00, 0x40,
        0x20, 0x40,
        0x40, 0x40,
        0x60, 0x40,
        0x80, 0x40,
        0xA0, 0x40,
        0xC0, 0x40,
        0xE0, 0x40,
        0x00, 0x60,
        0x20, 0x60,
        0x40, 0x60,
        0x60, 0x60,
        0x80, 0x60,
        0xA0, 0x60,
        0xC0, 0x60,
        0xE0, 0x60,
        0x00, 0x80,
        0x20, 0x80,
        0x40, 0x80,
        0x60, 0x80,
        0x80, 0x80,
        0xA0, 0x80,
        0xC0, 0x80,
        0xE0, 0x80,
        0x00, 0xA0,
        0x20, 0xA0,
        0x40, 0xA0,
        0x60, 0xA0,
        0x80, 0xA0,
        0xA0, 0xA0,
        0xC0, 0xA0,
        0xE0, 0xA0,
        0x00, 0xC0,
        0x20, 0xC0,
        0x40, 0xC0,
        0x60, 0xC0,
        0x80, 0xC0,
        0xA0, 0xC0,
        0xC0, 0xC0,
        0xE0, 0xC0,
        0x00, 0xE0,
        0x20, 0xE0,
        0x40, 0xE0,
        0x60, 0xE0,
        0x80, 0xE0,
        0xA0, 0xE0,
        0xC0, 0xE0,
        0xE0, 0xE0
    };

    public static readonly ushort[] D_BD154 = new ushort[] {
        0x0380, 0x0000, 0x0000, 0x0000,
        0x0380, 0x0000, 0x0080, 0x0000,
        0x03C0, 0x0000, 0x0000, 0x0000,
        0x03C0, 0x0000, 0x0080, 0x0000,
        0x0380, 0x0000, 0x0000, 0x0080,
        0x0380, 0x0000, 0x0080, 0x0080,
        0x03C0, 0x0000, 0x0000, 0x0080,
        0x03C0, 0x0000, 0x0080, 0x0080,
        0x0380, 0x0100, 0x0000, 0x0000,
        0x0380, 0x0100, 0x0080, 0x0000,
        0x03C0, 0x0100, 0x0000, 0x0000,
        0x03C0, 0x0100, 0x0080, 0x0000,
        0x0380, 0x0100, 0x0000, 0x0080,
        0x0380, 0x0100, 0x0080, 0x0080,
        0x03C0, 0x0100, 0x0000, 0x0080,
        0x03C0, 0x0100, 0x0080, 0x0080,
        0x0000, 0x0030, 0x0060, 0x0090,
        0x0030, 0x3030, 0x3060, 0x3090
    };

    public static readonly short[] D_BD214 = new short[] {
        1, -1,
        -1, -1,
        1, 1,
        -1, 1
    };

    public static byte D_44FE0_ST0;
    public static byte D_44FE1_ST1;
    public static byte D_44FE2_ST2;
    public static byte D_44FE3_ST3;

    public static short[] D_C5778;
    public static byte[] D_C57A8;
    public static byte[] D_C5E58;
    public static byte[] D_D5040;
    public static byte[] D_D6A50;
    public static byte[] D_D7488;
    public static List<OBJECT> D_D7588_ObjTable;
    public static OBJECT Terrain;
    public static int[] D_D7D88;
    public static byte[] D_D7E50;
    public static byte[] D_D8280;
    public static byte[] D_D8340;
    public static byte[] D_D8478;
    public static short[] D_D85B0;
    public static byte[] D_D8740;

    public static short D_BED9A_camRotY = 0x0c00;
    public static int D_D6A80_camPosX = 0x019800;
    public static int D_D6A88_camPosZ = 0x010400;

    public static byte[] D_C5FD0_texmapData;
    public static byte[] D_D50C0_heighmapData;
}

