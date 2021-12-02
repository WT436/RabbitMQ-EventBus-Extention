### ví dụ

1. Cài đặt
	- docker, rabbitServer , RabbitClient......
	- 
2. Cấu hình
	Chỗi kết nối : 
	```
	 string rabbitmqconnection = $"amqp://{HttpUtility.UrlEncode("UserName")}"
                                      + $":{ HttpUtility.UrlEncode("Pass")}"
                                      + $"@{ "host"}"
                                      + $":{"port"}"
                                      + $"/{ HttpUtility.UrlEncode("Visualhost")}";
	```
 Trong đó : 
 + UserName : tài khoản
 + Pass : mật khẩu
 <br/> ![image](https://user-images.githubusercontent.com/63473793/144386433-aecc9e2c-aa66-4f59-8de5-296b55c1deea.png)
 + host : địa chỉ máy chủ
 + port : cổng máy chủ
 + Visualhost : Máy chủ ảo - đi từ tài khoản vào
  <br/> ![image](https://user-images.githubusercontent.com/63473793/144387137-1312d741-65a8-4647-96cd-1092eb8b8257.png)
  <br/> ![image](https://user-images.githubusercontent.com/63473793/144387292-96cdb1ea-2ae1-48f2-9898-dc05be28cec9.png)
  <br/> Set cho nó
 => 
 ```
  string rabbitmqconnection = $"amqp://{HttpUtility.UrlEncode("WT436Admin")}"
                                      + $":{ HttpUtility.UrlEncode("WT436Admin")}"
                                      + $"@{ "localhost"}"
                                      + $":{"5672"}"
                                      + $"/{ HttpUtility.UrlEncode("TranHaiNam")}";
 ```
 
3. Test