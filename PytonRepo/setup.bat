@echo off

REM UTF-8 코드 페이지 설정
chcp 65001 >nul

REM Python 빌드 실행
echo [INFO] 새로 빌드 중...
"C:\Users\yemi7\anaconda3\envs\mg\python.exe" setup.py build_ext --inplace
IF %ERRORLEVEL% NEQ 0 (
    echo [ERROR] 빌드에 실패했습니다.
    pause
    exit /b %ERRORLEVEL%
)

REM 빌드된 파일 복사
echo [INFO] 빌드가 성공적으로 완료되었습니다. 파일을 복사합니다.
xcopy /y /q "*.pyd" "../"

REM 완료 메시지
echo [INFO] 파일 복사가 완료되었습니다.

REM 빌드된 파일 삭제
echo [INFO] 빌드 파일을 삭제합니다.
if exist "*.pyd" (
    del /q *.pyd
    echo [INFO] 빌드 파일이 삭제되었습니다.
) else (
    echo [INFO] 삭제할 빌드 파일이 없습니다.
)

pause
