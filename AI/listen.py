import socket

server_ip = "127.0.0.1" 
server_port = 12345     

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((server_ip, server_port))

server_socket.listen(1)

print("Waiting for a connection...")

client_socket, client_address = server_socket.accept()
print("Connection established with", client_address)

while True:
    data = client_socket.recv(1024)  
    if not data:
        continue
    game_state_json = data.decode('utf-8')
    print("Received game state data:")
    print(game_state_json)

client_socket.close()
server_socket.close()
