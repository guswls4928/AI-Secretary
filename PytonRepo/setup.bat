@echo off

REM UTF-8 코드 페이지 설정
chcp 65001 >nul

REM 지연된 변수 확장 활성화
setlocal enabledelayedexpansion

REM Python 빌드 실행
echo [INFO] 새로 빌드 중...
"C:\Users\yemi7\anaconda3\envs\mg\python.exe" setup.py build_ext --inplace
IF %ERRORLEVEL% NEQ 0 (
    echo [ERROR] 빌드에 실패했습니다.
    pause
    exit /b %ERRORLEVEL%
)

REM 빌드된 파일 이름 변경 및 복사
echo [INFO] 빌드가 성공적으로 완료되었습니다. 파일 이름을 변경하고 복사합니다.

for %%f in (*.pyd) do (
    echo [INFO] Processing file: %%~nf.pyd
    REM 접미사 제거 후 이름 변경 및 복사
    set newname=%%~nf.pyd
    set newname=!newname:.cp312-win_amd64=!
    echo "!newname!" | findstr /i "Client" >nul
    if !errorlevel! equ 0 (
        copy "%%~f" "..\Client\Assets\Dlls\!newname!"
        echo [INFO] Copied to Client Directory
    ) else (
        REM 그렇지 않은 경우 기본 디렉토리로 복사
        copy "%%~f" "..\Server\Server\DLLs\!newname!"
        echo [INFO] Copied to Default Directory
    )
)

REM 완료 메시지
echo [INFO] 파일 복사가 완료되었습니다.

REM 빌드된 파일 삭제
echo [INFO] 빌드 파일을 삭제합니다.
if exist "*.pyd" (
    del /q *.pyd
    del /q *.c
    rmdir /s /q "build"
    echo [INFO] 빌드 파일이 삭제되었습니다.
) else (
    echo [INFO] 삭제할 빌드 파일이 없습니다.
)

pause
