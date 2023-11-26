# CodeGeneratorYasarKusku

Merhabalar

Bugün sizlere kendi yazdığım basit ama etkili ve güvenilir bir CodeGeneratorYK yı tanıtmak istiyorum. Basit ve etkili, güvenilir olmasının en büyük sebebi Microsoft'un Cli kodlarıyla yazmış olmamdır. Peki bu proje bizim için ne yapıyor;

Öncelikle proje oluşurken bizden  
1-Proje Adı istiyor. 
2. Aşamada dosya yolu seçmemimizi istiyor(Örneğin projeyi D klasörüne oluşturmak istiyorsanız "d:" yazmanız yeterlidir.). 
3. Aşamada Entity Katmanın da kaç entity kullanacağınızı soruyor ve isimlerini istiyor.
4. Aşamada Migration yapıp veri tabanınıda oluşturmak isteyip istemediğinizi soruyor.

Peki bundan sonra neler oluyor;

1- Solution oluşturuyor
2- Ntier Katmanlı mimariye göre katmanları oluşturuyor
3- Katmanları Solution a bağlıyor
4- Katmanların referanslarını birbirine ekliyor.(Ntier mimarisine uygun olarak.)
5- NTier mimariye göre Abstract ve Concrete klasörlerini oluşturuyor
6- Sizin seçtiğiniz Entity'e göre View ve Controller dahil olmak üzere bütün class ları oluşturuyor.
7- IEntity, IEntityRepository, EntityRepositoryBase, DBContext classlarını oluşturup içlerini düzenliyor.
8- Controller ve tüm katmanlarda esnek bağlılığı sağlıyor ve program.cs altında Dİ katmanında esnek bağlılıklarını kendi ekliyor.
9- EntityFramework eklentilerini Data ve Business Katmanlarına ekliyor.(Katmanların hata vermemesi ve projenin direk çalışabilmesi adına seçim seçeneği sunulmamıştır.)
10- Proje oluşurken Migration istiyorum seçeneğini seçerseniz. Migration için name istiyor ve Migration oluşturuyor ve Update Database yapıyor.

Bize de projeyi açıp, çalıştırmak kalıyor :))),

Peki en önemli sorulardan bir tanesi; Proje Güncellemede yapıyor mu?
Eğer projeniz bu kodlar ile oluşturulduysa ya da .NetCore7 versiyonlarını kullanıyorsanız evet bu proje güncellemede yapabiliyor.
Yapmanız gereken projeAdını vermek Dosya yolunu seçmek ve istediğiniz entity leri yazmak. Dİ bağlılıklarını DBContex i güncelleyerek, Class View Controller ları ekliyor.

Projemiz VisualStudio2022 ve Core 7 versiyonunu kullanması sebebiyle uygulamayı kullanırken VisualStudio2022 yi kullanmak önemlidir. Yoksa kod çalışmayacaktır.
