# MangaMeeya - 만화 ZIP 뷰어

WPF(Windows Presentation Foundation)로 만든 만화 ZIP 파일 뷰어 애플리케이션입니다.

## 기능

- **ZIP 파일 열기**: 만화 ZIP 파일을 열고 자동으로 이미지 추출
- **이미지 표시**: 고품질 이미지 뷰어로 만화 페이지 표시
- **페이지 네비게이션**: 이전/다음 버튼으로 페이지 이동
- **페이지 정보**: 현재 페이지와 전체 페이지 수 표시
- **지원 포맷**: JPG, JPEG, PNG, BMP, GIF, WebP
- **모던 UI**: WPF 기반의 깔끔하고 반응형 사용자 인터페이스

## 설치 및 실행

### 요구사항
- .NET 6.0 이상
- Windows OS

### 빌드
```bash
dotnet build
```

### 실행
```bash
dotnet run
```

또는 exe 파일을 직접 실행합니다:
```bash
bin/Release/net6.0-windows/MangaMeeya.exe
```

## 사용 방법

1. **📁 ZIP 파일 열기** 버튼을 클릭합니다
2. 만화 ZIP 파일을 선택합니다
3. 자동으로 이미지가 추출되고 첫 페이지가 표시됩니다
4. **◀ 이전** / **다음 ▶** 버튼으로 페이지를 넘깁니다

## 프로젝트 구조

```
MangaMeeya_ByJ/
├── Program.cs              # 애플리케이션 진입점
├── App.xaml                # 애플리케이션 리소스
├── App.xaml.cs             # 애플리케이션 코드비하인드
├── MainWindow.xaml         # 메인 윈도우 UI
├── MainWindow.xaml.cs      # 메인 윈도우 로직
├── MangaMeeya.csproj       # 프로젝트 파일
└── README.md               # 이 파일
```

## 기술 스택

- **Framework**: .NET 6.0 WPF (Windows Presentation Foundation)
- **라이브러리**: SharpZipLib (ZIP 파일 처리)
- **UI**: XAML 기반 선언형 UI
- **언어**: C#

## 주요 기능 설명

### ZIP 파일 처리
- SharpZipLib를 사용하여 ZIP 파일 추출
- 임시 폴더에 자동으로 압축 해제
- 종료 시 자동 정리

### 이미지 렌더링
- BitmapImage를 사용한 효율적인 이미지 로딩
- Uniform 스트레칭으로 종횡비 유지
- 검은 배경에서 최적의 만화 감상 경험

### 네비게이션
- 직관적인 버튼 기반 페이지 이동
- 현재 페이지 정보 실시간 표시
- 첫/마지막 페이지 경계 처리

## 라이선스

MIT License
