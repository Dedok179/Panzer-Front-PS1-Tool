using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class PZ_Map
{
    private static List<PZ.OBJECT> globalObjData;
    private static List<PZ_Model.PositionData> globalPosData;

    public static void LoadResources(byte[] baFile, byte[] bbFile, int mapID)
    {
        using (BufferedBinaryReader bb = new BufferedBinaryReader(bbFile))
        {
            using (BufferedBinaryReader ba = new BufferedBinaryReader(baFile))
            {
                int terrainPtr = mapID * PZ.MAP_TERRAIN_CHUNK_SZ;
                int texturePtr = mapID * PZ.MAP_TEXTURES_CHUNK_SZ;
                int hmPtr = terrainPtr + 0x40C7;
                int tmPtr = hmPtr + 0x10000;
                int otherPtr = tmPtr + 0x10000;

                PZ_Texture.InitVRAM();

                //load object textures
                ba.Seek(texturePtr, SeekOrigin.Begin);
                PZ_Texture.LoadImage4Bit(ba, 896, 128, 512, 512);

                //load terrain textures
                ba.Seek(texturePtr + 0x20000, SeekOrigin.Begin);
                PZ_Texture.LoadImage4Bit(ba, 320, 256, 1024, 256);

                //load terrain palette
                ba.Seek(texturePtr + 0x40000, SeekOrigin.Begin);
                PZ_Texture.LoadImage16Bit(ba, 0, 480, 256, 32);

                //load skybox palette
                ba.Seek(texturePtr + 0x4A000, SeekOrigin.Begin);
                var skyboxPalette = PZ_Texture.LoadPalette16Bit(ba, 256);

                //load skybox texture
                ba.Seek(texturePtr + 0x44000, SeekOrigin.Begin);
                PZ_Texture.skyboxData = PZ_Texture.LoadImage(ba, 256, 96, 1, skyboxPalette);

                PZ.D_44FE0_ST0 = bb.ReadByte(terrainPtr);
                PZ.D_44FE1_ST1 = bb.ReadByte(terrainPtr + 0x4);
                PZ.D_44FE2_ST2 = bb.ReadByte(terrainPtr + 0x5);
                PZ.D_44FE3_ST3 = bb.ReadByte(terrainPtr + 0x6);

                //load data chunks
                PZ.D_D7D88 = new int[0x30];

                bb.Seek(terrainPtr + 0x7, SeekOrigin.Begin);

                for (int i = 0; i < 0x30; i++)
                {
                    PZ.D_D7D88[i] = (int)bb.Position;

                    bb.Seek(bb.ReadInt32() << 0x5, SeekOrigin.Current);
                }

                bb.Seek(0, SeekOrigin.Begin);

                PZ.D_D6A50 = bb.ReadBytes(0x30, otherPtr);
                PZ.D_D7E50 = bb.ReadBytes(0x30, otherPtr + 0x30);
                PZ.D_D8340 = bb.ReadBytes(0x30, otherPtr + 0x60);
                PZ.D_D8740 = bb.ReadBytes(0x30, otherPtr + 0x90);
                PZ.D_C5E58 = bb.ReadBytes(0x30, otherPtr + 0xC0);
                PZ.D_C5778 = bb.ReadShorts(0x18, otherPtr + 0xF0);
                PZ.D_C57A8 = bb.ReadBytes(0x30, otherPtr + 0x120);
                PZ.D_D5040 = bb.ReadBytes(0x30, otherPtr + 0x150);
                PZ.D_D8478 = bb.ReadBytes(0x30, otherPtr + 0x180);

                PZ.D_D7488 = bb.ReadBytes(0x100, otherPtr + 0x1B0);
                PZ.D_D8280 = bb.ReadBytes(0xC0, otherPtr + 0x2B0);

                PZ.D_D85B0 = new short[0x200];
                PZ.D_D7588_ObjTable = new List<PZ.OBJECT>();

                PZ.D_D50C0_heighmapData = bb.ReadBytes(256 * 256, hmPtr);
                PZ.D_C5FD0_texmapData = bb.ReadBytes(256 * 256, tmPtr);

                //load objects data
                short renderIndex = 0;

                for (int g = 0; g < 0x30; g++)
                {
                    short modelId = (short)g;

                    for (int subId = 0; subId < 4; subId++)
                    {
                        PZ.D_D85B0[modelId * 4 + subId] = renderIndex;

                        byte objCount = PZ.D_D8280[modelId * 4 + subId];

                        //if (objCount != 0)
                        {
                            for (int i = 0; i < objCount; i++)
                            {
                                if (renderIndex < 0x1FF)
                                {
                                    LoadObject(bb, modelId, subId, PZ.D_D7588_ObjTable, 0);

                                    renderIndex++;
                                }
                            }
                        }
                    }
                }                
            }
        }
    }

    public static void DrawTerrain(int cValue)
    {
        int tileX = -PZ.D_D6A80_camPosX + 0x200ff;

        if(tileX < 0)
            tileX = -PZ.D_D6A80_camPosX + 0x202fe;

        tileX >>= 0x9;

        int tileZ = -PZ.D_D6A88_camPosZ + 0x200ff;

        if (tileZ < 0)
            tileZ = -PZ.D_D6A88_camPosZ + 0x202fe;

        tileZ >>= 0x9;

        int mapBasePtr = tileZ * 256 + tileX;

        int polyCounter = 0;

        int rIdx;
        int coordZ2;
        int coordZ1;
        int coordX2;
        int coordX1;
        int rowOffset;
        int cIdx;
        int cMax;

        PZ.Terrain = new PZ.OBJECT();
        globalObjData = new List<PZ.OBJECT>();
        globalPosData = new List<PZ_Model.PositionData>();

        switch (cValue)
        {
            case 0: //OG render test (front)
                rIdx = 3;

                coordZ1 = 0x700;
                coordZ2 = 0x500;
                rowOffset = 0x300;

                int z = tileZ + 3;

                do {
                    cMax = rIdx + 4;
                    cIdx = 0;
                    int mapOffsetPtr = mapBasePtr + rowOffset;

                    if (0 < cMax)
                    {
                        coordX1 = 0x100;
                        coordX2 = -0x100;

                        do {
                            if (0x2CF < polyCounter) break;

                            if ((((uint)(tileX + cIdx) < 0xff) && (-1 < z)) && (z < 0xff))
                            {                                
                                PZ.Terrain.POLYGONS.Add(new PZ.FT4() {VERTICES = new int[] { coordX2, 0, coordZ2, coordX1, 0, coordZ2, coordX2, 0, coordZ1, coordX1, 0, coordZ1 } });
                                SetTerrainTile(PZ.D_C5FD0_texmapData[mapOffsetPtr], mapOffsetPtr, (short)polyCounter);
                            }
                            else
                            {
                                //null poly                             
                            }
                            
                            polyCounter++;
                            coordX1 -= 0x200;
                            coordX2 -= 0x200;
                            cIdx--;
                            mapOffsetPtr--;
                        } while (-cIdx < cMax);
                    }

                    cIdx = 1;

                    mapOffsetPtr = mapBasePtr + rowOffset + 1;

                    if (1 < cMax)
                    {
                        coordX1 = 0x300;
                        coordX2 = 0x100;

                        do {                          
                            if (0x2CF < polyCounter) break;

                            if ((((uint)(tileX + cIdx) < 0xff) && (-1 < z)) && (z < 0xff))
                            {
                                PZ.Terrain.POLYGONS.Add(new PZ.FT4() { VERTICES = new int[] { coordX2, 0, coordZ2, coordX1, 0, coordZ2, coordX2, 0, coordZ1, coordX1, 0, coordZ1 } });
                                SetTerrainTile(PZ.D_C5FD0_texmapData[mapOffsetPtr], mapOffsetPtr, (short)polyCounter);
                            }
                            else
                            {
                                //null poly
                            }
                            
                            polyCounter++;
                            coordX1 += 0x200;
                            coordX2 += 0x200;
                            cIdx++;
                            mapOffsetPtr++;
                        } while (cIdx < cMax);
                    }
                    
                    coordZ1 += 0x200;
                    coordZ2 += 0x200;
                    z++;
                    rowOffset += 0x100;
                    rIdx++;
                } while (polyCounter < 0x2D0);              
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 10: //full map (non-OG)
                tileX = 0;
                tileZ = 0;

                mapBasePtr = tileZ * 256 + tileX;
                rowOffset = 0;

                coordZ1 = -0xFE00;
                coordZ2 = -0x10000;

                for (int i = 0; i < 255; i++)
                {
                    coordX1 = -0xFE00;
                    coordX2 = -0x10000;

                    for (int j = 0; j < 255; j++)
                    {
                        int mapOffsetPtr = mapBasePtr + rowOffset + j;

                        PZ.Terrain.POLYGONS.Add(new PZ.FT4() { VERTICES = new int[] { coordX2, 0, coordZ2, coordX1, 0, coordZ2, coordX2, 0, coordZ1, coordX1, 0, coordZ1 } });                       
                        SetTerrainTile(PZ.D_C5FD0_texmapData[mapOffsetPtr], mapOffsetPtr, polyCounter);

                        coordX1 += 0x200;
                        coordX2 += 0x200;
                        polyCounter++;
                    }

                    coordZ1 += 0x200;
                    coordZ2 += 0x200;
                    rowOffset += 0x100;
                }
                break;
        } 
    }

    private static void SetTerrainTile(int tmByte, int hmPtr, int polyId)
    {
        PZ.Terrain.POLYGONS[polyId].VERTICES[1] = (short)(ushort)((PZ.D_D50C0_heighmapData[hmPtr] & 0xFC) * PZ.D_44FE0_ST0);
        PZ.Terrain.POLYGONS[polyId].VERTICES[4] = (short)(ushort)((PZ.D_D50C0_heighmapData[hmPtr + 1] & 0xFC) * PZ.D_44FE0_ST0);
        PZ.Terrain.POLYGONS[polyId].VERTICES[7] = (short)(ushort)((PZ.D_D50C0_heighmapData[hmPtr + 0x100] & 0xFC) * PZ.D_44FE0_ST0);
        PZ.Terrain.POLYGONS[polyId].VERTICES[10] = (short)(ushort)((PZ.D_D50C0_heighmapData[hmPtr + 0x101] & 0xFC) * PZ.D_44FE0_ST0);

        int tm = tmByte & 0xFF;
        int uvFlag = PZ.D_D50C0_heighmapData[hmPtr] & 3;
        int uvPtr = (tmByte & 0x3F) * 2;

        byte u = PZ.MAP_UV_TABLE[uvPtr];
        byte v = PZ.MAP_UV_TABLE[uvPtr + 1];

        if (PZ.D_D7488[tm] != 0xFF)
            SetObject(tm, uvFlag, hmPtr, polyId);

        short tPage = PZ.MAP_TPAGE_TABLE[tm >> 6];
        int cPage = ((((uvPtr >> 6) + 0x1F0) * 0x40) & 0xFFF0) + (tm >> 4);

        PZ.Terrain.POLYGONS[polyId].TPAGE = tPage;
        PZ.Terrain.POLYGONS[polyId].CPAGE = (short)cPage;

        int u0 = 0, u1 = 0, u2 = 0, u3 = 0;
        int v0 = 0, v1 = 0, v2 = 0, v3 = 0;

        switch (uvFlag)
        {
            case 0:
                u0 = u;
                v0 = v;
                u1 = u + 0x1F;
                v1 = v;
                u2 = u;
                v2 = v + 0x1F;
                u3 = u + 0x1F;
                v3 = v + 0x1F;

                PZ_Texture.ReColorImageByTerrain(tPage, cPage, u0, v0, 32, 32);
                break;
            case 1:
                u0 = u + 0x1F;
                v0 = v;
                u1 = u;
                v1 = v;
                u2 = u + 0x1F;
                v2 = v + 0x1F;
                u3 = u;
                v3 = v + 0x1F;

                PZ_Texture.ReColorImageByTerrain(tPage, cPage, u1, v1, 32, 32);
                break;
            case 2:
                u0 = u;
                v0 = v + 0x1F;
                u1 = u + 0x1F;
                v1 = v + 0x1F;
                u2 = u;
                v2 = v;
                u3 = u + 0x1F;
                v3 = v;

                PZ_Texture.ReColorImageByTerrain(tPage, cPage, u2, v2, 32, 32);
                break;
            case 3:
                u0 = u + 0x1F;
                v0 = v + 0x1F;
                u1 = u;
                v1 = v + 0x1F;
                u2 = u + 0x1F;
                v2 = v;
                u3 = u;
                v3 = v;

                PZ_Texture.ReColorImageByTerrain(tPage, cPage, u3, v3, 32, 32);
                break;
            default:
                break;
        }

        PZ.Terrain.POLYGONS[polyId].UVS = new byte[] { (byte)u0, (byte)v0, (byte)u1, (byte)v1, (byte)u2, (byte)v2, (byte)u3, (byte)v3 };
    }

    private static void SetObject(int tm, int uvFlag, int hmPtr, int polyId)
    {
        int index = PZ.D_D7488[tm & 0xFF];
        int offset = uvFlag + index * 4;

        int objId = PZ.D_D85B0[offset];
        byte cDt = PZ.D_D5040[index];

        byte hm1 = 0;
        byte hm2 = 0;

        int posX = PZ.Terrain.POLYGONS[polyId].VERTICES[9] - 0x100;
        int posY = 0;      
        int posZ = PZ.Terrain.POLYGONS[polyId].VERTICES[11] - 0x100;

        if ((cDt == 0x0) || (cDt == 0x5) || (cDt == 0x6))
        {
            if (9999 < PZ.D_C5778[index / 2])
            {
                posY = 0;

                goto ended;
            }

            hm1 = PZ.D_D50C0_heighmapData[hmPtr + 0x1];
            hm2 = PZ.D_D50C0_heighmapData[hmPtr + 0x100];
        }
        else
        {
            if (((cDt == 0x1 && (uvFlag < 2)) || ((cDt == 0x2 && ((byte)(uvFlag - 2) < 2)))))
            {
                posY = (short)(((PZ.D_D50C0_heighmapData[hmPtr - 0xFF] & 0xFC) + (PZ.D_D50C0_heighmapData[hmPtr] & 0xFC) >> 1) * PZ.D_44FE0_ST0);
            }

            if (((PZ.D_D5040[index] == 0x1) && ((byte)(uvFlag - 2) < 2)) || ((PZ.D_D5040[index] == 0x2 && (uvFlag < 2))))
            {
                posY = (short)(((PZ.D_D50C0_heighmapData[hmPtr + 0x101] & 0xFC) + (PZ.D_D50C0_heighmapData[hmPtr + 0x200] & 0xFC) >> 1) * PZ.D_44FE0_ST0);
            }

            if (((PZ.D_D5040[index] == 0x3) && ((uvFlag == 0 || (uvFlag == 2)))) || ((PZ.D_D5040[index] == 0x4 && ((uvFlag == 1 || (uvFlag == 3))))))
            {
                posY = (short)(((PZ.D_D50C0_heighmapData[hmPtr] & 0xFC) + (PZ.D_D50C0_heighmapData[hmPtr + 0xFF] & 0xFC) >> 1) * PZ.D_44FE0_ST0);
            }

            if (((PZ.D_D5040[index] == 0x3) || ((uvFlag != 1 && (uvFlag != 3)))) && ((PZ.D_D5040[index] == 0x4 || ((uvFlag != 0 && (uvFlag != 2))))))
                goto ended;

            hm1 = PZ.D_D50C0_heighmapData[hmPtr + 0x2];
            hm2 = PZ.D_D50C0_heighmapData[hmPtr + 0x101];
        }

        posY = (short)(((hm1 & 0xFC) + (hm2 & 0xFC) >> 1) * PZ.D_44FE0_ST0);

        float fposX = (float)(posX << 8) / PZ.TRANSLATION_FACTOR;
        float fposY = (float)(posY << 8) / PZ.TRANSLATION_FACTOR;
        float fposZ = (float)(posZ << 8) / PZ.TRANSLATION_FACTOR;

        globalObjData.Add(PZ.D_D7588_ObjTable[objId]);
        globalPosData.Add(new PZ_Model.PositionData(fposX, fposY, fposZ));

    ended:
        return;
    }

    private static void LoadObject(BufferedBinaryReader bb, short modelId, int subId, List<PZ.OBJECT> objTable, int flag)
    {
        PZ.OBJECT modelObj = new PZ.OBJECT();

        bb.Seek(PZ.D_D7D88[modelId], SeekOrigin.Begin);

        int polyCount = bb.ReadInt32();
        int subOffset = ((subId << 0x10) >> 0xE) / 2;

        short sX = PZ.D_BD214[subOffset];
        short sZ = PZ.D_BD214[subOffset + 1];

        for (int i = 0; i < polyCount; i++)
        {
            PZ.FT4 poly = new PZ.FT4();

            short[] verts = bb.ReadShorts(0xC);
            byte[] uvs = bb.ReadBytes(0x8);

            int tId = PZ.D_D6A50[modelId];

            poly.TPAGE = (short)((ushort)((PZ.D_BD154[(tId * 4) + 1] & 0x100) >> 0x4) | (ushort)((PZ.D_BD154[(tId * 4)] & 0x3FF) >> 0x6) | (ushort)((PZ.D_BD154[(tId * 4) + 1] & 0x200) << 0x2));
            poly.CPAGE = (short)(tId & 0x3F | 0x7800);

            byte uBase = (byte)(PZ.D_BD154[(tId * 4) + 2] & 0xFF);
            byte vBase = (byte)(PZ.D_BD154[(tId * 4) + 3] & 0xFF);
            
            bool flipPoly = (ushort)(subId - 1) >= 2;

            for (int v = 0; v < 0xC; v += 3)
            {
                verts[v] *= sX;
                verts[v + 2] *= sZ;
            }

            for (int u = 0; u < 4; u++)
            {
                uvs[u] += uBase;
                uvs[u + 4] += vBase;
            }

            if (!flipPoly)
            {
                poly.UVS = uvs;
                
                poly.VERTICES[0] = verts[0];
                poly.VERTICES[1] = verts[1];
                poly.VERTICES[2] = verts[2];

                poly.VERTICES[3] = verts[3];
                poly.VERTICES[4] = verts[4];
                poly.VERTICES[5] = verts[5];

                poly.VERTICES[6] = verts[6];
                poly.VERTICES[7] = verts[7];
                poly.VERTICES[8] = verts[8];

                poly.VERTICES[9] = verts[9];
                poly.VERTICES[10] = verts[10];
                poly.VERTICES[11] = verts[11];
            }
            else
            {
                poly.UVS[1] = uvs[0];
                poly.UVS[0] = uvs[1];
                poly.UVS[3] = uvs[2];
                poly.UVS[2] = uvs[3];

                poly.UVS[5] = uvs[4];
                poly.UVS[4] = uvs[5];
                poly.UVS[7] = uvs[6];
                poly.UVS[6] = uvs[7];
                
                poly.VERTICES[3] = verts[0];
                poly.VERTICES[4] = verts[1];
                poly.VERTICES[5] = verts[2];

                poly.VERTICES[0] = verts[3];
                poly.VERTICES[1] = verts[4];
                poly.VERTICES[2] = verts[5];

                poly.VERTICES[9] = verts[6];
                poly.VERTICES[10] = verts[7];
                poly.VERTICES[11] = verts[8];

                poly.VERTICES[6] = verts[9];
                poly.VERTICES[7] = verts[10];
                poly.VERTICES[8] = verts[11];
            }

            PZ_Texture.ReColorImageByObject(poly.TPAGE, poly.CPAGE, poly.UVS);

            modelObj.POLYGONS.Add(poly);
        }

        objTable.Add(modelObj);

        bb.Seek(0, SeekOrigin.Begin);
    }

    public static void ExportMap(string path)
    {
        PZ_Model.Dae exporter = new PZ_Model.Dae();
        PZ_Model.MeshesData terrainData = new PZ_Model.MeshesData();

        terrainData.modelIdx.Add(0);

        terrainData.vertices.Add(0, new List<double>());
        terrainData.normals.Add(0, new List<double>());
        terrainData.uvs.Add(0, new List<double>());
        terrainData.indices.Add(0, new Dictionary<short, List<string>>());

        int c = 0;

        foreach (var poly in PZ.Terrain.POLYGONS)
        {
            terrainData.vertices[0].AddRange(new double[] { (float)(poly.VERTICES[0] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[1] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[2] << 8) / PZ.TRANSLATION_FACTOR });
            terrainData.vertices[0].AddRange(new double[] { (float)(poly.VERTICES[3] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[4] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[5] << 8) / PZ.TRANSLATION_FACTOR });
            terrainData.vertices[0].AddRange(new double[] { (float)(poly.VERTICES[6] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[7] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[8] << 8) / PZ.TRANSLATION_FACTOR });
            terrainData.vertices[0].AddRange(new double[] { (float)(poly.VERTICES[9] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[10] << 8) / PZ.TRANSLATION_FACTOR, (float)(poly.VERTICES[11] << 8) / PZ.TRANSLATION_FACTOR });

            if (!terrainData.indices[0].ContainsKey(poly.TPAGE))
                terrainData.indices[0].Add(poly.TPAGE, new List<string>());

            if (!terrainData.materials.ContainsKey(poly.TPAGE))
            {
                terrainData.materials.Add(poly.TPAGE, poly.TPAGE);
                terrainData.materialIdx.Add(poly.TPAGE);
            }

            terrainData.uvs[0].AddRange(new double[] { (float)poly.UVS[0] / 255, 1f - (float)poly.UVS[1] / 255 });
            terrainData.uvs[0].AddRange(new double[] { (float)poly.UVS[2] / 255, 1f - (float)poly.UVS[3] / 255 });
            terrainData.uvs[0].AddRange(new double[] { (float)poly.UVS[4] / 255, 1f - (float)poly.UVS[5] / 255 });
            terrainData.uvs[0].AddRange(new double[] { (float)poly.UVS[6] / 255, 1f - (float)poly.UVS[7] / 255 });

            terrainData.indices[0][poly.TPAGE].Add($"{c} {c} {c + 1} {c + 1} {c + 2} {c + 2} ");
            terrainData.indices[0][poly.TPAGE].Add($"{c + 1} {c + 1} {c + 3} {c + 3} {c + 2} {c + 2} ");

            c += 4;
        }

        exporter.Export(path, terrainData, "TERRAIN");      
    }

    public static void ExportObjects(string path)
    {
        PZ_Model.Dae exporter = new PZ_Model.Dae();
        PZ_Model.MeshesData modelsData = new PZ_Model.MeshesData();
        
        for (int i = 0; i < globalObjData.Count; i++)
        {
            PZ.OBJECT modelObj = globalObjData[i];
        
            int m = i;
        
            modelsData.modelIdx.Add(m);
        
            modelsData.vertices.Add(m, new List<double>());
            modelsData.normals.Add(m, new List<double>());
            modelsData.uvs.Add(m, new List<double>());
            modelsData.indices.Add(m, new Dictionary<short, List<string>>());
        
            int cc = 0;
        
            for (int p = 0; p < modelObj.POLYGONS.Count; p++)
            {
                for (int v = 0; v < 0xC; v += 3)
                {
                    float x = (float)(modelObj.POLYGONS[p].VERTICES[v] << 8) / PZ.TRANSLATION_FACTOR;
                    float y = (float)(modelObj.POLYGONS[p].VERTICES[v + 1] << 8) / PZ.TRANSLATION_FACTOR;
                    float z = (float)(modelObj.POLYGONS[p].VERTICES[v + 2] << 8) / PZ.TRANSLATION_FACTOR;
        
                    modelsData.vertices[m].AddRange(new double[] { x, y, z });
                }
        
                if (!modelsData.indices[m].ContainsKey(modelObj.POLYGONS[p].TPAGE))
                    modelsData.indices[m].Add(modelObj.POLYGONS[p].TPAGE, new List<string>());
        
                if (!modelsData.materials.ContainsKey(modelObj.POLYGONS[p].TPAGE))
                {
                    modelsData.materials.Add(modelObj.POLYGONS[p].TPAGE, modelObj.POLYGONS[p].TPAGE);
                    modelsData.materialIdx.Add(modelObj.POLYGONS[p].TPAGE);
                }
        
                float u0_ = (float)modelObj.POLYGONS[p].UVS[0] / 255;
                float u1_ = (float)modelObj.POLYGONS[p].UVS[1] / 255;
                float u2_ = (float)modelObj.POLYGONS[p].UVS[2] / 255;
                float u3_ = (float)modelObj.POLYGONS[p].UVS[3] / 255;
        
                float v0_ = 1f - (float)modelObj.POLYGONS[p].UVS[4] / 255;
                float v1_ = 1f - (float)modelObj.POLYGONS[p].UVS[5] / 255;
                float v2_ = 1f - (float)modelObj.POLYGONS[p].UVS[6] / 255;
                float v3_ = 1f - (float)modelObj.POLYGONS[p].UVS[7] / 255;
        
                modelsData.uvs[m].AddRange(new double[] { u0_, v0_ });
                modelsData.uvs[m].AddRange(new double[] { u1_, v1_ });
                modelsData.uvs[m].AddRange(new double[] { u2_, v2_ });
                modelsData.uvs[m].AddRange(new double[] { u3_, v3_ });
        
                modelsData.indices[m][modelObj.POLYGONS[p].TPAGE].Add($"{cc + 2} {cc + 2} {cc + 1} {cc + 1} {cc} {cc} ");
                modelsData.indices[m][modelObj.POLYGONS[p].TPAGE].Add($"{cc + 2} {cc + 2} {cc + 3} {cc + 3} {cc + 1} {cc + 1} ");
        
                cc += 4;
            }
        }

        exporter.Export(path, modelsData, "OBJECT", globalPosData.ToArray());
    }

}

