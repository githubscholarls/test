docker stop webapitest
docker rm webapitest
git pull
docker build -t dockerfilewebapitest -f DockerfileWebApiTest  .
docker run -itd -v /root/volumes/webapitest/upload:/app/upload -v /root/volumes/webapitest/tmp:/app/tmp --net mytestnet --ip 172.18.0.3 -p 18030:8080 --name  webapitest dockerfilewebapitest
