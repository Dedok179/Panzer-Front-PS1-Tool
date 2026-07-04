using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Collada141;

public static class PZ_Model
{
    public class MeshesData
    {
        public MeshesData()
        {
            vertices = new Dictionary<int, List<double>>();
            uvs = new Dictionary<int, List<double>>();
            normals = new Dictionary<int, List<double>>();
            indices = new Dictionary<int, Dictionary<short, List<string>>>();
            materials = new Dictionary<short, short>();
            modelIdx = new List<int>();
            materialIdx = new List<short>();
        }

        public Dictionary<int, List<double>> vertices;
        public Dictionary<int, List<double>> uvs;
        public Dictionary<int, List<double>> normals;
        public Dictionary<int, Dictionary<short, List<string>>> indices;
        public Dictionary<short, short> materials;
        public List<int> modelIdx;
        public List<short> materialIdx;
    }

    public class PositionData 
    {
        public PositionData(double x, double y, double z) { X = x; Y = y; Z = z; }

        public readonly double X;
        public readonly double Y;
        public readonly double Z;
    }

    public class Dae 
    {
        public Dae()
        {
        }

        public void Export(string filename, MeshesData meshes, string space, PositionData[] positions = null)
        {
            var _asset = new asset
            {
                contributor = new assetContributor[]
                {
                    new assetContributor
                    {
                        author = "Dedok179",
                        authoring_tool = "Panzer Front PS1 Tool"
                    }
                },
                created = DateTime.Now,
                modified = DateTime.Now,
                up_axis = UpAxisType.Y_UP
            };

            var images = new library_images
            {
                image = new image[meshes.materialIdx.Count]
            };

            var materials = new library_materials
            {
                material = new material[meshes.materialIdx.Count]
            };

            var effects = new library_effects
            {
                effect = new effect[meshes.materialIdx.Count]
            };

            var geometries = new library_geometries
            {
                geometry = new geometry[meshes.modelIdx.Count]
            };

            var visualSceneNodes = new node[meshes.modelIdx.Count];

            var visualScenes = new library_visual_scenes
            {
                visual_scene = new[]
                {
                        new visual_scene
                        {
                            id = "Scene",
                            name = "Scene",
                            node = visualSceneNodes
                        }
                }
            };

            #region TEXTURES
            for (int i = 0; i < meshes.materialIdx.Count; i++)
            {
                var tex_name = $"TEX_{meshes.materials[meshes.materialIdx[i]].ToString("X2")}";
                var mat_name = $"MAT_{meshes.materials[meshes.materialIdx[i]].ToString("X2")}";

                images.image[i] = new image
                {
                    id = tex_name,
                    name = tex_name,
                    Item = $"{tex_name}.png"
                };

                materials.material[i] = new material
                {
                    id = mat_name,
                    name = mat_name,

                    instance_effect = new instance_effect
                    {
                        url = $"#{mat_name}-fx"
                    }
                };

                var surface = new common_newparam_type
                {
                    sid = $"{tex_name}-surface",
                    ItemElementName = ItemChoiceType.surface,

                    Item = new fx_surface_common
                    {
                        type = fx_surface_type_enum.Item2D,

                        init_from = new fx_surface_init_from_common[]
                        {
                            new fx_surface_init_from_common
                            {
                                Value = tex_name
                            }
                        }
                    }
                };

                var sampler = new common_newparam_type
                {
                    sid = $"{tex_name}-sampler",
                    ItemElementName = ItemChoiceType.sampler2D,

                    Item = new fx_sampler2D_common
                    {
                        source = $"{tex_name}-surface"
                    }
                };

                effects.effect[i] = new effect
                {
                    id = $"{mat_name}-fx",

                    Items = new effectFx_profile_abstractProfile_COMMON[]
                    {
                        new effectFx_profile_abstractProfile_COMMON
                        {
                            Items = new Object[]
                            {
                                surface,
                                sampler,
                            },

                            technique = new effectFx_profile_abstractProfile_COMMONTechnique
                            {
                                sid = "common",

                                Item = new effectFx_profile_abstractProfile_COMMONTechniqueLambert
                                {
                                    emission = new common_color_or_texture_type
                                    {
                                        Item = new common_color_or_texture_typeColor
                                        {
                                            sid = "emission",
                                            _Text_ = "0 0 0 1"
                                        }
                                    },

                                    diffuse = new common_color_or_texture_type
                                    {
                                        Item = new common_color_or_texture_typeTexture
                                        {
                                            texture = $"{tex_name}-sampler",
                                            texcoord = "UVMap"
                                        }
                                    },

                                    index_of_refraction = new common_float_or_param_type
                                    {
                                        Item = new common_float_or_param_typeFloat
                                        {
                                            sid = "ior",
                                            Value = 1.45
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
            #endregion

            #region MODELS
            var acessorVert = new[]
            {
                new param
                {
                    name = "X",
                    type = "float"
                },
                new param
                {
                    name = "Y",
                    type = "float"
                },
                new param
                {
                    name = "Z",
                    type = "float"
                }
            };

            var acessorUv = new[]
            {
                new param
                {
                    name = "S",
                    type = "float"
                },
                new param
                {
                    name = "T",
                    type = "float"
                }
            };

            for (int i = 0; i < meshes.modelIdx.Count; i++)
            {
                var model_name = $"{space}_{meshes.modelIdx[i].ToString("")}";

                var positionAccessor = new accessor
                {
                    count = (ulong)meshes.vertices[meshes.modelIdx[i]].Count / 3,
                    offset = 0,
                    source = $"#{model_name}-mesh-positions-array",
                    stride = 3,
                    param = acessorVert
                };

                var positionTechnique = new sourceTechnique_common
                {
                    accessor = positionAccessor
                };

                var positionArray = new float_array
                {
                    id = $"{model_name}-mesh-positions-array",
                    count = (ulong)meshes.vertices[meshes.modelIdx[i]].Count,
                    Values = meshes.vertices[meshes.modelIdx[i]].ToArray()
                };

                var positionSource = new source
                {
                    id = $"{model_name}-mesh-positions",
                    Item = positionArray,
                    technique_common = positionTechnique
                };

                var uvAccessor = new accessor
                {
                    count = (ulong)meshes.uvs[meshes.modelIdx[i]].Count / 2,
                    offset = 0,
                    source = $"#{model_name}-mesh-map-array",
                    stride = 2,
                    param = acessorUv
                };

                var uvTechnique = new sourceTechnique_common
                {
                    accessor = uvAccessor
                };

                var uvArray = new float_array
                {
                    id = $"{model_name}-mesh-map-array",
                    count = (ulong)meshes.uvs[meshes.modelIdx[i]].Count,
                    Values = meshes.uvs[meshes.modelIdx[i]].ToArray()
                };

                var uvSource = new source
                {
                    id = $"{model_name}-mesh-map",
                    Item = uvArray,
                    technique_common = uvTechnique
                };

                var indc = meshes.indices[meshes.modelIdx[i]].Count;
                var keys = meshes.indices[meshes.modelIdx[i]].Keys.ToList();

                var triangles = new triangles[indc];
                var instanceMat = new instance_material[keys.Count];

                for (int j = 0; j < indc; j++)
                {
                    string tri = "";

                    for (int t = 0; t < meshes.indices[meshes.modelIdx[i]][keys[j]].Count; t++)
                        tri += meshes.indices[meshes.modelIdx[i]][keys[j]][t];                        

                    instanceMat[j] = new instance_material
                    {
                        symbol = "MAT_" + keys[j].ToString("X2"),
                        target = "#MAT_" + keys[j].ToString("X2"),

                        bind_vertex_input = new instance_materialBind_vertex_input[]
                        {
                            new instance_materialBind_vertex_input
                            {
                                semantic = "UVMap",
                                input_semantic = "TEXCOORD",
                                input_set = 0
                            }
                        }
                    };

                    triangles[j] = new triangles
                    {
                        count = (ulong)meshes.indices[meshes.modelIdx[i]][keys[j]].Count,
                        material = "MAT_" + keys[j].ToString("X2"),

                        input = new[]
                        {
                            new InputLocalOffset
                            {
                                offset = 0,
                                semantic = "VERTEX",
                                source = $"#{model_name}-mesh-vertices"
                            },

                            new InputLocalOffset
                            {
                                offset = 1,
                                semantic = "TEXCOORD",
                                source = $"#{model_name}-mesh-map"
                            }
                        },

                        p = tri
                    };
                }

                var mesh = new mesh
                {
                    source = new[] { positionSource, uvSource },

                    vertices = new vertices
                    {
                        id = $"{model_name}-mesh-vertices",

                        input = new[]
                        {
                            new InputLocal
                            {
                                semantic = "POSITION",
                                source = $"#{model_name}-mesh-positions"
                            }
                        }
                    },

                    Items = triangles
                };

                var geometry = new geometry
                {
                    id = $"{model_name}-mesh",
                    name = model_name,
                    Item = mesh
                };

                geometries.geometry[i] = geometry;

                var x = 0.0;
                var y = 0.0;
                var z = 0.0;

                if (positions != null) 
                {
                    x = positions[i].X;
                    y = positions[i].Y;
                    z = positions[i].Z;
                }

                var matrix_pos = new double[] { 1, 0, 0, x, 0, 1, 0, y, 0, 0, 1, z, 0, 0, 0, 1 };

                visualSceneNodes[i] = new node
                {
                    id = model_name,
                    name = model_name,
                    type = NodeType.NODE,
                    ItemsElementName = new[] { ItemsChoiceType2.matrix },

                    Items = new Object[]
                    {
                        new matrix
                        {
                            sid = "transform",
                            Values = matrix_pos
                        },
                    },

                    instance_geometry = new instance_geometry[]
                    {
                        new instance_geometry
                        {
                            url = $"#{model_name}-mesh",
                            name = model_name,

                            bind_material = new bind_material
                            {
                                technique_common = instanceMat
                            }
                        }
                    }
                };
            }
            #endregion

            var m = new COLLADA
            {
                asset = _asset,

                Items = new Object[]
                {
                    images,
                    materials,
                    effects,
                    geometries,
                    visualScenes
                },

                scene = new COLLADAScene
                {
                    instance_visual_scene = new InstanceWithExtra
                    {
                        url = "#Scene"
                    }
                }
            };

            m.Save(filename);
        }
    }

    public static void GetTankModels(byte[] data, string dir, bool exportAll, int tankId) 
    {
        string oDir = dir;

        if (!Directory.Exists(oDir))
        {
            Directory.CreateDirectory(oDir);
        }

        Dae daeExporter = new Dae();

        using (BufferedBinaryReader r = new BufferedBinaryReader(data)) 
        {
            int chunkId = tankId;
            int chunkCount = 1;

            if (exportAll == true) { chunkId = 0; chunkCount = (data.Length / PZ.TANK_SZ) / 0x3; }  //0x89           

            for (int i = chunkId; i < chunkId + chunkCount; i++) 
            {
                for (int p = 0; p < 3; p++)
                {
                    r.Seek(((i * 3) + p) * PZ.TANK_SZ, SeekOrigin.Begin);

                    List<PositionData> positions = new List<PositionData>();

                    MeshesData modelsData = new MeshesData();                                       

                    int meshPtr = ((i * 3) + p) * PZ.TANK_SZ + 0x4;
                    int texPtr = meshPtr + PZ.TANK_MODEL_SZ;
                    int palPtr = texPtr + PZ.TANK_TEXTURE_SZ;
                    int posPtr = palPtr + PZ.TANK_PALETTE_SZ;

                    int modelsCount = r.ReadInt16();
                    r.ReadInt16(); //unk count

                    for (byte m = 0; m < modelsCount; m++)
                    {
                        modelsData.modelIdx.Add(m);

                        modelsData.vertices.Add(m, new List<double>());
                        modelsData.normals.Add(m, new List<double>());
                        modelsData.uvs.Add(m, new List<double>());
                        modelsData.indices.Add(m, new Dictionary<short, List<string>>());

                        int c = 0;

                        uint blockCount = r.ReadUInt32();

                        for (int b = 0; b < blockCount; b++)
                        {
                            for (int v = 0; v < 4; v++)
                            {
                                float x = (float)(r.ReadInt16() << 8) / PZ.TRANSLATION_FACTOR;
                                float y = (float)(r.ReadInt16() << 8) / PZ.TRANSLATION_FACTOR;
                                float z = (float)(r.ReadInt16() << 8) / PZ.TRANSLATION_FACTOR;

                                modelsData.vertices[m].AddRange(new double[] { x, y, z });
                            }

                            if (!modelsData.indices[m].ContainsKey(0))
                                modelsData.indices[m].Add(0, new List<string>());

                            if (!modelsData.materials.ContainsKey(0))
                            {
                                modelsData.materials.Add(0, 0);
                                modelsData.materialIdx.Add(0);
                            }

                            float u0 = (float)r.ReadByte() / (PZ.TANK_TEXTURE_W - 1);
                            float u1 = (float)r.ReadByte() / (PZ.TANK_TEXTURE_W - 1);
                            float u2 = (float)r.ReadByte() / (PZ.TANK_TEXTURE_W - 1);
                            float u3 = (float)r.ReadByte() / (PZ.TANK_TEXTURE_W - 1);

                            float v0 = 1f - (float)r.ReadByte() / (PZ.TANK_TEXTURE_H - 1);
                            float v1 = 1f - (float)r.ReadByte() / (PZ.TANK_TEXTURE_H - 1);
                            float v2 = 1f - (float)r.ReadByte() / (PZ.TANK_TEXTURE_H - 1);
                            float v3 = 1f - (float)r.ReadByte() / (PZ.TANK_TEXTURE_H - 1);

                            modelsData.uvs[m].AddRange(new double[] { u0, v0 });
                            modelsData.uvs[m].AddRange(new double[] { u1, v1 });
                            modelsData.uvs[m].AddRange(new double[] { u2, v2 });
                            modelsData.uvs[m].AddRange(new double[] { u3, v3 });

                            modelsData.indices[m][0].Add($"{c + 2} {c + 2} {c + 1} {c + 1} {c} {c} ");
                            modelsData.indices[m][0].Add($"{c + 2} {c + 2} {c + 3} {c + 3} {c + 1} {c + 1} ");

                            c += 4;
                        }

                        long ret = r.Position;

                        r.Seek(posPtr + (m * 0xC), SeekOrigin.Begin);
                        
                        float pX = (float)(r.ReadInt32() << 8) / PZ.TRANSLATION_FACTOR;
                        float pY = (float)(r.ReadInt32() << 8) / PZ.TRANSLATION_FACTOR;
                        float pZ = (float)(r.ReadInt32() << 8) / PZ.TRANSLATION_FACTOR;
                        
                        positions.Add(new PositionData(-pX, -pY, -pZ));

                        r.Seek(ret, SeekOrigin.Begin);
                    }
                                      
                    string mDir = oDir + Path.DirectorySeparatorChar + $"{i}_" + PZ.TANK_NAME_TABLE[i] + Path.DirectorySeparatorChar + $"{p}" + Path.DirectorySeparatorChar;

                    if (!Directory.Exists(mDir))
                    {
                        Directory.CreateDirectory(mDir);
                    }

                    r.Seek(palPtr, SeekOrigin.Begin);

                    Color[] pal = PZ_Texture.GetTankPalette(r);

                    r.Seek(texPtr, SeekOrigin.Begin);

                    PZ_Texture.GetTankTextures(mDir, r, pal);

                    daeExporter.Export($"{mDir}{PZ.TANK_NAME_TABLE[i]}_{p}.dae", modelsData, "TRANSPORT", positions.ToArray());
                }
            }
        }
    }
}

