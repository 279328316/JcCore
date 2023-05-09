echo off
cls
:: 设置变量不能有空格 等号左边不允许有空格，等号右边的所有包括空格会全部赋值给变量。
set version=1.1.6
echo User Dir %USERPROFILE%   FileVersion : %version%
echo Target Dir %USERPROFILE%\.nuget\packages\jc.core\%version%\lib\netstandard2.0
xcopy /E/I/Y "Jc.Base\bin\Debug\netstandard2.0" "%USERPROFILE%\.nuget\packages\jc.base\%version%\lib\netstandard2.0"
xcopy /E/I/Y "Jc.Core\bin\Debug\netstandard2.0" "%USERPROFILE%\.nuget\packages\jc.core\%version%\lib\netstandard2.0"
xcopy /E/I/Y "Jc.Core.Database\bin\Debug\netstandard2.0" "%USERPROFILE%\.nuget\packages\jc.core.database\%version%\lib\netstandard2.0"
echo Copy Finished.
pause 