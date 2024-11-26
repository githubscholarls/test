docker stop webapitest
docker rm webapitest
git pull
docker build -t dockerfilewebapitest -f DockerfileWebApiTest  .
docker run -itd --net mytestnet --ip 172.18.0.3 -p 18030:8080 --name  webapitest dockerfilewebapitest
