import json
import random
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

    # action = random.choice(['move_forward', 'move_back', 'move_left', 'move_right'])
    # client_socket.send(action.encode('utf-8'))
    # print(f"Send {action}")
    vertical_movement = random.uniform(0, 1)
    horizontal_movement = random.uniform(-1, 1)
    movement_json = json.dumps({"verticalMovement": vertical_movement, 
                                "horizontalMovement": horizontal_movement})

    client_socket.send(movement_json.encode('utf-8'))


client_socket.close()
server_socket.close()
