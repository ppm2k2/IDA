REM Copy Dev version of web.config
cd E:\Development\Configuration
svn update --username axiomainc\svc-svnadmin --password tRekaf7e
xcopy "E:\Development\Configuration\QA (Dev Data)\IDA\DEMI\web.config" "\\C1DEVWEB01.CONCEPTONELLC.COM\Applications$\Current\DEMI\Web.config" /y
cd E:\Development\IDA
