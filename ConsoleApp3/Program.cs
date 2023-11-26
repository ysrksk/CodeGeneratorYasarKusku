using System.Diagnostics;
using System.Globalization;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Programın Deneme Sürümüne Hoşgeldiniz...");

            DateTime gecerlilikTarihi = new DateTime(2023, 12, 30);
            DateTime anlik = DateTime.Now;
            var kalanGun = gecerlilikTarihi - anlik;

            if (gecerlilikTarihi > anlik)
            {
                Console.WriteLine($"Program kullanımı için kalan gün sayınız: {kalanGun.Days} Gün");
                Console.WriteLine("...");
                Console.WriteLine("...");

                #region Proje Adı

                Console.WriteLine("Oluşturmak istediğiniz proje adını giriniz.");
                string projeName = Console.ReadLine();

                while (projeName == null || projeName == "")
                {
                    Console.WriteLine("Lütfen geçerli bir proje adı giriniz...");
                    string projeName1 = Console.ReadLine();
                    if (projeName1 != null)
                    {
                        projeName = projeName1;
                    }
                }
                #endregion

                #region Dosya Yolu
                Console.WriteLine("Projeyi Oluşturmak İstediğiniz Klasör'ün Dosya Yolunu Giriniz");
                Console.WriteLine("Öğreğin: D:\\Dosyalarım");
                Console.WriteLine("Standart olarak Örnekteki klasör'e oluşsun istiyorsanız örnekteki dosya yolunu yazınız. ");
                string projeSolutionFile = Console.ReadLine();

                while (projeSolutionFile == null || projeSolutionFile == "")
                {
                    Console.WriteLine("Lütfen dosya yolunu boş bırakmayınız...");
                    string projeSolutionFile1 = Console.ReadLine();
                    if (projeSolutionFile != null)
                    {
                        projeSolutionFile = projeSolutionFile1;
                    }
                }

                while (!Directory.Exists(projeSolutionFile))
                {
                    Console.WriteLine("Lütfen bilgisayarınızda bulunan bir dosya yolu giriniz...");
                    string projeSolutionFile1 = Console.ReadLine();
                    if (Directory.Exists(projeSolutionFile1))
                    {
                        projeSolutionFile = projeSolutionFile1;
                    }
                }


                projeSolutionFile = projeSolutionFile.Trim();
                #endregion

                #region Entity
                Console.WriteLine("Kaç adet entity oluşturmak istiyorsunuz.");

                int entitySayi = 1;
                var success = Int32.TryParse(Console.ReadLine(), out entitySayi);
                while (success == false)
                {
                    Console.WriteLine("Lütfen Geçerli Bir Sayı Giriniz...");
                    var success1 = Int32.TryParse(Console.ReadLine(), out entitySayi);
                    if (success1)
                    {
                        success = true;
                    }
                }

                List<string> list = new List<string>();

                for (int i = 1; i <= entitySayi; i++)
                {
                    Console.WriteLine($"Oluşturmak istediğiniz {i}. Entity'i giriniz. ");

                    var girilenEntity = Console.ReadLine().ToLower();
                    bool entityName1 = true;
                    if (girilenEntity == "") { entityName1 = false; }
                    while (entityName1 == false)
                    {
                        Console.WriteLine("Lütfen Geçerli Bir İsim Giriniz...");
                        girilenEntity = Console.ReadLine().ToLower();
                        if (girilenEntity != "") { entityName1 = true; }
                    }
                    girilenEntity = girilenEntity.Trim();
                    girilenEntity = girilenEntity.Replace(" ", "");
                    var girilenEntity1 = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(girilenEntity);
                    list.Add(girilenEntity1);
                }
                #endregion

                #region Migration
                Console.WriteLine("Bilgisayarınız da Sql Server yüklü ise Migration yapılmasını ister misiniz?");
                Console.WriteLine("1- İstemiyorum.        2- İstiyorum");

                int mig = 1;
                var migSuccess = Int32.TryParse(Console.ReadLine(), out mig);
                while (migSuccess == false || mig != 1 && mig != 2)
                {
                    Console.WriteLine("Lütfen Geçerli Bir Sayı Giriniz...");
                    Console.WriteLine("1- İstemiyorum.        2- İstiyorum");
                    var migSuccess1 = Int32.TryParse(Console.ReadLine(), out mig);
                    if (migSuccess1 && mig == 1 || mig == 2)
                    {
                        migSuccess = true;
                    }
                }
                #endregion

                using (Process process = new Process())
                {
                    #region Proje Klasörünü Oluştur
                    string klasorYolu = $"{projeSolutionFile}\\{projeName}";
                    if (!Directory.Exists(klasorYolu))
                    {
                        string powerShellCommand = $"New-Item -ItemType Directory -Path '{projeSolutionFile}\\{projeName}'";

                        ExecutePowerShellCommand(powerShellCommand);
                    }
                    #endregion

                    #region Solution Oluştur
                    string solutionName = projeName.ToUpper();
                    string dosyaYolu = @$"{projeSolutionFile}\{projeName}\{solutionName}.sln";

                    if (!File.Exists(dosyaYolu))
                    {
                        AddSolution($"{solutionName}", $"{projeSolutionFile}", $"{projeName}");
                    }
                    #endregion

                    #region Katmanların Oluşturulması
                    AddLayer($"{solutionName}.Business", $"{projeSolutionFile}", $"{projeName}", "Business", "classlib");
                    AddLayer($"{solutionName}.Data", $"{projeSolutionFile}", $"{projeName}", "Data", "classlib");
                    AddLayer($"{solutionName}.Entity", $"{projeSolutionFile}", $"{projeName}", "Entity", "classlib");
                    AddLayer($"{solutionName}.WebUi", $"{projeSolutionFile}", $"{projeName}", "WebUi", "mvc");
                    #endregion

                    #region Abstract ve Concrete Dosyalarının Oluşturulması
                    AddClass($"IEntity", $"{projeSolutionFile}", $"{projeName}", "Entity/Abstract", "interface");
                    AddClass($"IEntityRepository", $"{projeSolutionFile}", $"{projeName}", "Data/Abstract", "interface");
                    AddClass($"EfEntityRepositoryBase", $"{projeSolutionFile}", $"{projeName}", "Data/Concrete", "class");
                    AddClass($"{solutionName}Context", $"{projeSolutionFile}", $"{projeName}", "Data/Concrete", "class");

                    foreach (var entity in list)
                    {
                        AddClass($"I{entity}Service", $"{projeSolutionFile}", $"{projeName}", "Business/Abstract", "interface");
                        AddClass($"{entity}Manager", $"{projeSolutionFile}", $"{projeName}", "Business/Concrete", "class");
                        AddClass($"I{entity}Dal", $"{projeSolutionFile}", $"{projeName}", "Data/Abstract", "interface");
                        AddClass($"Ef{entity}Dal", $"{projeSolutionFile}", $"{projeName}", "Data/Concrete", "class");
                        AddClass($"{entity}", $"{projeSolutionFile}", $"{projeName}", "Entity/Concrete", "class");
                        AddClass($"{entity}Controller", $"{projeSolutionFile}", $"{projeName}", "WebUi/Controllers", "class");
                        AddClass($"Index", $"{projeSolutionFile}", $"{projeName}", $"WebUi/Views/{entity}", "page");
                        AddClass($"Add", $"{projeSolutionFile}", $"{projeName}", $"WebUi/Views/{entity}", "page");
                        AddClass($"Update", $"{projeSolutionFile}", $"{projeName}", $"WebUi/Views/{entity}", "page");
                    };
                    #endregion

                    #region Solution a katmanları ekle
                    AddProjectToSolution("Business", solutionName, projeSolutionFile, projeName);
                    AddProjectToSolution("WebUi", solutionName, projeSolutionFile, projeName);
                    AddProjectToSolution("Data", solutionName, projeSolutionFile, projeName);
                    AddProjectToSolution("Entity", solutionName, projeSolutionFile, projeName);
                    #endregion

                    #region Proje Referanslarının eklendiği Bölüm
                    AddReferance($"{solutionName}", $"{projeSolutionFile}", $"{projeName}", "Business", "Entity");
                    AddReferance($"{solutionName}", $"{projeSolutionFile}", $"{projeName}", "Business", "Data");
                    AddReferance($"{solutionName}", $"{projeSolutionFile}", $"{projeName}", "Data", "Entity");
                    AddReferance($"{solutionName}", $"{projeSolutionFile}", $"{projeName}", "WebUi", "Entity");
                    AddReferance($"{solutionName}", $"{projeSolutionFile}", $"{projeName}", "WebUi", "Business");
                    #endregion

                    #region EntityFrameWork Kurulumları
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore", "7.0.14", "Data");
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore.SqlServer", "7.0.14", "Data");
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore.Design", "7.0.14", "Data");
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore", "7.0.14", "Business");
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore.SqlServer", "7.0.14", "Business");
                    AddPackage($"{projeSolutionFile}", $"{projeName}", $"Microsoft.EntityFrameworkCore.Design", "7.0.14", "Business");
                    #endregion

                    #region Gereksiz Class temizleme
                    RemoveClass("Business", $"{projeSolutionFile}", $"{projeName}", "Class1.cs");
                    RemoveClass("Data", $"{projeSolutionFile}", $"{projeName}", "Class1.cs");
                    RemoveClass("Entity", $"{projeSolutionFile}", $"{projeName}", "Class1.cs");
                    foreach (var entity in list)
                    {
                        RemoveClass($"WebUi\\Views\\{entity}", $"{projeSolutionFile}", $"{projeName}", "Add.cshtml.cs");
                        RemoveClass($"WebUi\\Views\\{entity}", $"{projeSolutionFile}", $"{projeName}", "Update.cshtml.cs");
                        RemoveClass($"WebUi\\Views\\{entity}", $"{projeSolutionFile}", $"{projeName}", "Index.cshtml.cs");
                    }
                    #endregion

                    #region Abstract ve Concrete de  bulunan cs dosyalarının içlerinin düzenlenmesi
                    foreach (var entity in list)
                    {
                        //Entity Düzenleme
                        {

                            string dosyaYolu1 = @$"{projeSolutionFile}\{projeName}\Entity\Concrete\{entity}.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu1);
                                if (!dosyaIcerik.Contains("IEntity"))
                                {
                                    string baslangicMetni = $"namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"using System.ComponentModel.DataAnnotations;

namespace {solutionName}.Entity;

public class {entity} : IEntity
{{
    [Key]
     public int Id {{ get; set; }}
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu1, dosyaIcerik);
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }
                        };

                        //Controller Düzenleme
                        {
                            var entity1 = entity.ToLower();
                            string dosyaYolu1 = @$"{projeSolutionFile}\{projeName}\WebUi\Controllers\{entity}Controller.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu1);
                                if (!dosyaIcerik.Contains("Update"))
                                {
                                    string baslangicMetni = $"namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using {solutionName}.WebUi.Models;
using {solutionName}.Entity;
using {solutionName}.Business;

namespace {solutionName}.WebUi.Controllers;

public class {entity}Controller : Controller
{{
private readonly I{entity}Service _{entity1}Service;

public {entity}Controller(I{entity}Service {entity1}Service)
{{
     _{entity1}Service = {entity1}Service;
}}

    public IActionResult Index()
{{
     return View();
}}

public IActionResult Add({entity} {entity1})
{{
     _{entity1}Service.Add({entity1});
     return View();
}}

public IActionResult Update({entity} {entity1})
{{
     _{entity1}Service.Update({entity1});
     return View();
}}

public IActionResult Delete(int id)
{{
     //_{entity1}Service.Delete(id);
     return View();
}}
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu1, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                            //dotnet new interface -n I$entity -o Business\Abstract
                        };

                        //IEntityDal
                        {
                            string dosyaYolu4 = @$"{projeSolutionFile}\{projeName}\Data\Abstract\I{entity}Dal.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu4);
                                if (!dosyaIcerik.Contains("IEntityRepository"))
                                {
                                    string baslangicMetni = "namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"using {solutionName}.Entity;
using {solutionName}.Data;

namespace {solutionName}.Data
{{
    public interface I{entity}Dal : IEntityRepository<{entity}>
    {{
    }}
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu4, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                        };

                        //IBusinessService
                        {
                            string dosyaYolu6 = @$"{projeSolutionFile}\{projeName}\Business\Abstract\I{entity}Service.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu6);
                                if (!dosyaIcerik.Contains("Update"))
                                {
                                    string baslangicMetni = "namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"using {solutionName}.Entity;
using System.Linq.Expressions;

namespace {solutionName}.Business;

public interface I{entity}Service
{{
     void Add ({entity} entity);
     void Update ({entity} entity);
     void Delete ({entity} entity);
     {entity} GetById(int id);
     List<{entity}> GetAll(Expression<Func<{entity}, bool>> filter = null);
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu6, dosyaIcerik);
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                        };

                        //BusinessManager
                        {
                            var entity1 = entity.ToLower();
                            string dosyaYolu7 = @$"{projeSolutionFile}\{projeName}\Business\Concrete\{entity}Manager.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu7);
                                if (!dosyaIcerik.Contains("GetById"))
                                {
                                    string baslangicMetni = "namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"﻿using {solutionName}.Data;
using {solutionName}.Entity;
using System.Linq.Expressions;

namespace {solutionName}.Business;

public class {entity}Manager : I{entity}Service
{{
    private readonly I{entity}Dal _{entity1}Dal;

    public {entity}Manager(I{entity}Dal {entity1}Dal)
    {{
        _{entity1}Dal = {entity1}Dal;
    }}

    public void Add({entity} {entity1})
    {{
        _{entity1}Dal.Add({entity1});
    }}

    public void Delete({entity} {entity1})
    {{
        _{entity1}Dal.Delete({entity1});
    }}

    public {entity} GetById(int id)
    {{
        var entity = _{entity1}Dal.Get(x => x.Id == id);
        return entity;
    }}

    public List<{entity}> GetAll(Expression<Func<{entity}, bool>> filter = null)
    {{
        return _{entity1}Dal.GetAll(filter);
    }}

    public void Update({entity} {entity1})
    {{
        _{entity1}Dal.Update({entity1});
    }}
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu7, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                        }

                        //DataConcrete EfEntityDal İçerik Düzenleme
                        {
                            var entity1 = entity.ToLower();
                            string dosyaYolu8 = @$"{projeSolutionFile}\{projeName}\Data\Concrete\Ef{entity}Dal.cs";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu8);
                                if (!dosyaIcerik.Contains("EfEntityRepositoryBase"))
                                {
                                    string baslangicMetni = "namespace";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = $@"﻿using {solutionName}.Entity;

namespace {solutionName}.Data;

public class Ef{entity}Dal : EfEntityRepositoryBase<{entity}, {solutionName}Context>, I{entity}Dal
{{
}}";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu8, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                        }

                        //Views Add Düzenleme
                        {
                            string dosyaYolu1 = @$"{projeSolutionFile}\{projeName}\WebUi\Views\{entity}\Add.cshtml";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu1);
                                if (!dosyaIcerik.Contains("Update"))
                                {
                                    string baslangicMetni = $"@page";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = " ";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu1, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }
                        };

                        //Views Update Düzenleme
                        {
                            string dosyaYolu1 = @$"{projeSolutionFile}\{projeName}\WebUi\Views\{entity}\Update.cshtml";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu1);
                                if (!dosyaIcerik.Contains("table"))
                                {
                                    string baslangicMetni = $"@page";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = " ";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu1, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }
                        };

                        //Views Index Düzenleme
                        {
                            string dosyaYolu1 = @$"{projeSolutionFile}\{projeName}\WebUi\Views\{entity}\Index.cshtml";

                            try
                            {
                                // Dosyayı oku
                                string dosyaIcerik = File.ReadAllText(dosyaYolu1);
                                if (!dosyaIcerik.Contains("Table"))
                                {
                                    string baslangicMetni = $"@page";
                                    string bitisMetni = "}";
                                    // Değiştirmek istediğiniz kod parçasını belirtin
                                    int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                                    int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                                    // If the class is found, remove it
                                    if (baslangicIndex != -1 && bitisIndex != -1)
                                    {
                                        dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                                    }

                                    // Yeni kodu belirtin
                                    string yeniMetin = " ";

                                    // İçeriği değiştir
                                    dosyaIcerik += yeniMetin;
                                    // Değiştirilmiş içerikle dosyayı tekrar yaz
                                    File.WriteAllText(dosyaYolu1, dosyaIcerik);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }
                        };
                    };

                    //Entityframework Context

                    string dosyaYolu2 = @$"{projeSolutionFile}\{projeName}\Data\Concrete\{solutionName}Context.cs";
                    try
                    {
                        // Dosyayı oku
                        string dosyaIcerik = File.ReadAllText(dosyaYolu2);
                        if (!dosyaIcerik.Contains("DbSet"))
                        {
                            string baslangicMetni = "namespace";
                            string bitisMetni = "}";
                            // Değiştirmek istediğiniz kod parçasını belirtin
                            int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                            int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                            // If the class is found, remove it
                            if (baslangicIndex != -1 && bitisIndex != -1)
                            {
                                dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                            }
                            string dbSet = "";

                            foreach (var item in list)
                            {
                                dbSet += $"public DbSet<{item}> {item}s {{get; set; }}\n ";
                            }
                            // Yeni kodu belirtin
                            string yeniMetin = $@"﻿﻿using {solutionName}.Entity;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Data;

public class {solutionName}Context : DbContext
{{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {{
        optionsBuilder.UseSqlServer(""Server = localhost; Database = {solutionName}Db; Integrated Security = True; TrustServerCertificate = True"");
    }}
    
    {dbSet}
}}";

                            // İçeriği değiştir
                            dosyaIcerik += yeniMetin;
                            // Değiştirilmiş içerikle dosyayı tekrar yaz
                            File.WriteAllText(dosyaYolu2, dosyaIcerik);
                        }
                        else
                        {
                            string[] satirlar = File.ReadAllLines(dosyaYolu2);
                            int sonSatirIndex = satirlar.Length - 1;
                            // Eğer başlangıç ve bitiş metni bulunduysa, araya yeni metni ekleyin
                            string dbSet = "";
                            foreach (var item in list)
                            {
                                dbSet += $"public DbSet<{item}> {item}s {{get; set; }}\n ";
                            }

                            // Yeni kodu belirtin
                            string yeniMetin = $@"{dbSet}";

                            string yeniSatir = $"{yeniMetin}\n";

                            // Yeni satırı son satırın üzerine ekle
                            satirlar[sonSatirIndex - 1] += yeniSatir;

                            // Dosyayı güncelle
                            File.WriteAllLines(dosyaYolu2, satirlar);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + ex.Message);
                    }

                    //IEntityRepository
                    string dosyaYolu3 = @$"{projeSolutionFile}\{projeName}\Data\Abstract\IEntityRepository.cs";
                    try
                    {
                        // Dosyayı oku
                        string dosyaIcerik = File.ReadAllText(dosyaYolu3);
                        if (!dosyaIcerik.Contains("Update"))
                        {
                            string baslangicMetni = $"namespace {solutionName}.Data;";
                            string bitisMetni = "}";
                            // Değiştirmek istediğiniz kod parçasını belirtin
                            int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                            int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                            // If the class is found, remove it
                            if (baslangicIndex != -1 && bitisIndex != -1)
                            {
                                dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                            }

                            // Yeni kodu belirtin
                            string yeniMetin = $@"﻿using {solutionName}.Entity;
using System.Linq.Expressions;

namespace {solutionName}.Data
{{
	public interface IEntityRepository<T> where T : class, IEntity, new()
	{{
		T Get(Expression<Func<T, bool>> filter);
		List<T> GetAll(Expression<Func<T, bool>> filter = null);
		void Add(T entity);
		void Update(T entity);
		void Delete(T entity);
	}}
}}";

                            // İçeriği değiştir
                            dosyaIcerik += yeniMetin;
                            // Değiştirilmiş içerikle dosyayı tekrar yaz
                            File.WriteAllText(dosyaYolu3, dosyaIcerik);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + ex.Message);
                    }

                    //EfEntityRepositoryBase
                    string dosyaYolu5 = @$"{projeSolutionFile}\{projeName}\Data\Concrete\EfEntityRepositoryBase.cs";
                    try
                    {
                        // Dosyayı oku
                        string dosyaIcerik = File.ReadAllText(dosyaYolu5);

                        if (!dosyaIcerik.Contains("TEntity"))
                        {
                            string baslangicMetni = $"namespace {solutionName}.Data;";
                            string bitisMetni = "}";
                            // Değiştirmek istediğiniz kod parçasını belirtin
                            int baslangicIndex = dosyaIcerik.IndexOf(baslangicMetni);
                            int bitisIndex = dosyaIcerik.IndexOf(bitisMetni, baslangicIndex);

                            // If the class is found, remove it
                            if (baslangicIndex != -1 && bitisIndex != -1)
                            {
                                dosyaIcerik = dosyaIcerik.Remove(baslangicIndex, bitisIndex - baslangicIndex + bitisMetni.Length);
                            }

                            // Yeni kodu belirtin
                            string yeniMetin = $@"﻿using {solutionName}.Data;
using {solutionName}.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace {solutionName}.Data;

public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
{{
    public void Add(TEntity entity)
    {{
        using (TContext context = new TContext())
        {{
            var addedEntity = context.Entry(entity);
            addedEntity.State = EntityState.Added;
            context.SaveChanges();
        }}
    }}

    public void Delete(TEntity entity)
    {{
        using (TContext context = new TContext())
        {{
            var deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            context.SaveChanges();
        }}
    }}

    public TEntity Get(Expression<Func<TEntity, bool>> filter)
    {{
        using (TContext context = new TContext())
        {{
            return context.Set<TEntity>().SingleOrDefault(filter);
        }}
    }}

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
    {{
        using (TContext context = new TContext())
        {{
            return filter == null ? context.Set<TEntity>().ToList() : context.Set<TEntity>().Where(filter).ToList();
        }}
    }}

    public void Update(TEntity entity)
    {{
        using (TContext context = new TContext())
        {{
            var updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            context.SaveChanges();
        }}
    }}
}}";

                            // İçeriği değiştir
                            dosyaIcerik += yeniMetin;
                            // Değiştirilmiş içerikle dosyayı tekrar yaz
                            File.WriteAllText(dosyaYolu5, dosyaIcerik);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + ex.Message);
                    }

                    //Programcs düzenleme
                    string dosyaYolu9 = @$"{projeSolutionFile}\{projeName}\WebUi\Program.cs";
                    try
                    {
                        // Dosyayı oku
                        string dosyaIcerik = File.ReadAllText(dosyaYolu9);
                        string eskiMetin = "var builder = WebApplication.CreateBuilder(args);\r\n\r\n// Add services to the container.\r\nbuilder.Services.AddControllersWithViews();";
                        // Yeni servisleri ekle
                        var builder = "";
                        if (!eskiMetin.Contains("Manager"))
                        {
                            foreach (var item in list)
                            {
                                builder += $"builder.Services.AddScoped<I{item}Dal, Ef{item}Dal>();\n" +
                                    $"builder.Services.AddScoped<I{item}Service, {item}Manager>();\n";
                            }
                            // Yeni kodu belirtin
                            string yeniMetin = $@"﻿﻿using {solutionName}.Business;
using {solutionName}.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
{builder}";

                            // İçeriği değiştir
                            dosyaIcerik = dosyaIcerik.Replace(eskiMetin, yeniMetin);
                            // Değiştirilmiş içerikle dosyayı tekrar yaz
                            File.WriteAllText(dosyaYolu9, dosyaIcerik);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + ex.Message);
                    }
                    #endregion

                    #region Migration
                    if (mig == 2)
                    {
                        Console.WriteLine("Migration eklemek için lütfen bir migration ismi giriniz.");
                        var migName = Console.ReadLine().ToLower();
                        if (migName == null)
                        {
                            migName = "initialCreate";
                        }

                        AddMiration($"{projeSolutionFile}", $"{projeName}", $"{migName}");
                        UpdateDatabase($"{projeSolutionFile}", $"{projeName}");
                    }
                    #endregion
                }
            }
            else
            {
                Console.WriteLine("Programı Kullanım Süsreniz Dolmuştur. Yaşar KÜSKÜ ile iletişime geçebilirsiniz.");
                Console.ReadKey();
            }

            #region Methods
            static void AddProjectToSolution(string projectName, string solutionName, string projeSolutionFile, string projeName)
            {
                using (Process process = new Process())
                {
                    Console.WriteLine($"{projectName} Katmanı Solution' Ekleniyor");
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"sln {solutionName}.sln add {projectName}";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                    process.Start();
                    process.WaitForExit();
                }
            }

            static void ExecutePowerShellCommand(string command)
            {
                try
                {
                    // PowerShell processini başlat
                    using (Process process = new Process())
                    {
                        // PowerShell'i çalıştırma işlemleri
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };

                        process.StartInfo = startInfo;
                        process.Start();

                        // Komutu PowerShell'e gönder
                        process.StandardInput.WriteLine(command);
                        process.StandardInput.Flush();
                        process.StandardInput.Close();

                        // Çıktıları oku
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

                        // Hata kontrolü
                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine("Hata: " + error);
                        }
                        else
                        {
                            Console.WriteLine("Komut başarıyla çalıştırıldı.");
                        }

                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Bir hata oluştu: " + ex.Message);
                }
            }

            static void AddLayer(string fileName, string projeSolutionFile, string projeName, string layer, string dosyaTuru)
            {
                var solutionName = projeName.ToUpper();
                string dosyaYolu = @$"{projeSolutionFile}\{projeName}\{layer}\{solutionName}.{layer}.csproj";

                if (!File.Exists(dosyaYolu))
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "dotnet";
                        process.StartInfo.Arguments = $"new {dosyaTuru} -n {fileName} -o {layer}";
                        process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                        process.Start();
                        process.WaitForExit();
                    }
                }
            }

            static void RemoveClass(string fileRoad, string projeSolutionFile, string projeName, string className)
            {
                using (Process process = new Process())
                {
                    string dosyaYolu = @$"{projeSolutionFile}\{projeName}\{fileRoad}\{className}";

                    if (File.Exists(dosyaYolu))
                    {
                        string projectPath = @$"{projeSolutionFile}\{projeName}\{fileRoad}";
                        process.StartInfo.FileName = "powershell.exe";
                        process.StartInfo.Arguments = $"/c del {projectPath}\\{className}";
                        process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();
                    }
                }
            }

            static void AddPackage(string projeSolutionFile, string projeName, string packageName, string versionName, string katman)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"add package {packageName} --version {versionName}";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}\{katman}";
                    process.Start();
                    process.WaitForExit();
                }
            }

            static void AddReferance(string solutionName, string projeSolutionFile, string projeName, string addedLayer, string willAddLayer)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "powershell.exe";
                    process.StartInfo.Arguments = @$"dotnet add {projeSolutionFile}\{projeName}\{addedLayer}\{solutionName}.{addedLayer}.csproj reference {projeSolutionFile}\{projeName}\{willAddLayer}\{solutionName}.{willAddLayer}.csproj";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                }
            }

            static void AddSolution(string solutionName, string projeSolutionFile, string projeName)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"new sln -n {solutionName}";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                    process.Start();
                    process.WaitForExit();
                }
            }

            static void AddClass(string fileName, string projeSolutionFile, string projeName, string layer, string dosyaTuru)
            {
                var solutionName = projeName.ToUpper();
                string dosyaYolu;
                if (dosyaTuru == "page")
                {
                    dosyaYolu = @$"{projeSolutionFile}\{projeName}\{layer}\{fileName}.cshtml";
                }
                else
                {
                    dosyaYolu = @$"{projeSolutionFile}\{projeName}\{layer}\{fileName}.cs";
                }

                if (!File.Exists(dosyaYolu))
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "dotnet";
                        process.StartInfo.Arguments = $"new {dosyaTuru} -n {fileName} -o {layer}";
                        process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}";
                        process.Start();
                        process.WaitForExit();
                    }
                }
            }

            static void AddMiration(string projeSolutionFile, string projeName, string migName)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"ef migrations add {migName}";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}\Data";
                    process.Start();
                    process.WaitForExit();
                }
            }

            static void UpdateDatabase(string projeSolutionFile, string projeName)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "dotnet";
                    process.StartInfo.Arguments = $"ef database update";
                    process.StartInfo.WorkingDirectory = @$"{projeSolutionFile}\{projeName}\Data";
                    process.Start();
                    process.WaitForExit();
                }
            }
            #endregion
        }
    }
}


