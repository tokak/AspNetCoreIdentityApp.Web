# ASP.NET Core Kimlik Doğrulama ve Yetkilendirme Projesi

Bu proje, ASP.NET Core üzerinde geliştirilmiş, modern kimlik doğrulama ve yetkilendirme özelliklerine sahip, N katmanlı mimari ile yapılandırılmış bir web uygulamasıdır.

## Özellikler

🔐 **Kimlik Doğrulama ve Üyelik İşlemleri**  
- Üye girişi ve çıkışı  
- Üye olma, şifremi unuttum ve şifre sıfırlama işlemleri  
- Şifre güncelleme ve kullanıcı bilgilerini görüntüleme  

📩 **E-posta Doğrulama**  
- Girişte e-posta doğrulama kodu ile ikinci adım doğrulama

📱 **İki Faktörlü Kimlik Doğrulama**  
- Microsoft Authenticator ve Google Authenticator uygulamaları ile 2FA desteği

🌐 **Sosyal Medya ile Giriş**  
- Facebook, Google ve Microsoft hesapları ile kimlik doğrulama

🛡️ **Yetkilendirme**  
- Rol tabanlı yetkilendirme  
- Claim bazlı yetkilendirme  
- Rol oluşturma, güncelleme, silme ve atama işlemleri  
- Web sayfaları için yetki kontrolü

🏗️ **Mimari Yapı**  
- N katmanlı mimari (Entity, Data Access, Business, UI)  
- Katmanlar arası temiz bağımlılık prensibi  
- Katmanlara göre ayrılmış validasyon ve hata mesajları (IPasswordValidator, IUserValidator, IdentityErrorDescriber)

## Kullanılan Teknolojiler
- ASP.NET Core Identity
- Entity Framework Core
- ASP.NET MVC 
- Microsoft Identity Platform
- Google ve Facebook Authentication
- Two-Factor Authentication (2FA)
- Katmanlı Mimari

📸 Ekran Görüntüleri
<div align="center"> <table> <tr> <td><img src="https://github.com/user-attachments/assets/fbe3bbc0-84ca-4f90-957f-2d047a35e8c7" width="400" alt="Giriş Ekranı"/></td> <td><img src="https://github.com/user-attachments/assets/4dea989b-58ba-4100-a73c-523698fb00ec" width="400" alt="E-posta Doğrulama"/></td> </tr> <tr> <td><img src="https://github.com/user-attachments/assets/c51e47e7-b215-4df8-a5a6-80e26d353504" width="400" alt="2FA"/></td> <td><img src="https://github.com/user-attachments/assets/28c519b8-7ce6-46ee-b655-1a9e73cf6f68" width="400" alt="Sosyal Giriş"/></td> </tr> <tr> <td><img src="https://github.com/user-attachments/assets/826171d3-e465-49ce-9c56-8d73f0ea01f3" width="400" alt="Yetkilendirme"/></td> <td><img src="https://github.com/user-attachments/assets/ad5687c8-78a0-426d-81ba-6aa8c0cc66f3" width="400" alt="Mimari Yapı"/></td> </tr> </table> </div>






