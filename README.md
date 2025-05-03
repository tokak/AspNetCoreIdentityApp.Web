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
