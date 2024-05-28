echo off
cls
:: 设置变量不能有空格 等号左边不允许有空格，等号右边的所有包括空格会全部赋值给变量。
set version=1.2.1.4
echo User Dir %USERPROFILE%   FileVersion : %version%

set curDir="%USERPROFILE%\.nuget\packages\jc.base\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Base\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core.database\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core.Database\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core.mssql\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core.MsSql\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core.mysql\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core.MySql\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core.postgresql\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core.PostgreSql\bin\Debug\netstandard2.0" %curDir%

set curDir="%USERPROFILE%\.nuget\packages\jc.core.sqlite\%version%\lib\netstandard2.0\"
echo Target Dir : %curDir%
Del "%curDir%\*" /q
xcopy /Y "Jc.Core.Sqlite\bin\Debug\netstandard2.0" %curDir%

echo Copy Finished.
pause 