docker build -f "D:\Workspace\Lambot\src\Lambot.Template\Dockerfile" --force-rm -t lambottemplate  --build-arg "BUILD_CONFIGURATION=Debug" --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=Lambot.Template" "D:\Workspace\Lambot"