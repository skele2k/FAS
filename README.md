# FAS
FAS - Fingerprint Attendance System

The project contains two seperate applications and one web api. One for scanning and updating database, and another for modifying users.  

How to setup WEB-API:
  1. Go to Control-Panel
  2. View by-> Category
  3. Uninstall a program (Don't worry we're not gonna uninstall a program)
  4. Go to "Turn Windows features on or off"
  5. Check "Internet Information Services" (I will refer "Internet Information Services by IIS from now on)
  6. Check the following:  
      IIS -> World Wide Web Services -> Application Development Features -> Check ASP.NET 4.8  
      IIS -> World Wide Web Services -> Common HTTP Features -> Check except WebDAV  
      IIS -> World Wide Web Services -> Health and Diagnosis -> Check HTTP Logging  
      IIS -> World Wide Web Services -> Check Performance features  
      IIS -> World Wide Web Services -> Security -> Check Request Filtering  

  7. Open "Windows Defender Firewall with Advanced Security"  
      -> New Rule  
      -> Select Port  
      -> TCP/Specific Local Ports  
      -> Write 8888  
      -> Allow Connection  
      -> Next  
      -> Next  
      -> Finish  
  8. Open Internet Information Services (IIS) Manager  
      -> In the Connections pane, right-click the Sites node in the tree, and then click Add Website.  
      -> Enter Website name(fas)  
      -> In the Physical path, give the path to the API folder  
      -> In Port, Write 8888  
      -> Click OK.   
  9. Open Internet Information Services (IIS) Manager   
      -> Go to Application Pool  
      -> fas(Given API Name)  
      -> Advanced Settings  
        -> Set Identity to "Local System"  
        -> Set 32-bit Application to "True"  
All done. The WEB Api has been set to your local network.  
Try searching "http://yourip:8888"  


Download FAS Admin: https://drive.google.com/file/d/1Dr5Aa_gXM7Bap4RolIYz5vvkutjosA_p/view?usp=sharing  
Download FAS Desktop: https://drive.google.com/file/d/1jNaahes0r-HSR3-l0jOrshYzpunQDHYm/view?usp=sharing  
Download FAS API: https://drive.google.com/file/d/1hlbbI_BnvKJnhTcyJMkrg3z8uUkNTG4b/view?usp=sharing  
