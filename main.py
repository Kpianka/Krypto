import os
import time
from stegano import lsb
from stegano.lsb import generators
import argparse

def hide_message(image_path, output_path, secret_message, generator):
    start_time = time.time()
    try:
        secret_image = lsb.hide(image_path, secret_message, generator)
        secret_image.save(output_path)
        end_time = time.time()
        print(f"Message hidden in {output_path}")
        original_size = os.path.getsize(image_path)
        new_size = os.path.getsize(output_path)
        print(f"Original size: {original_size} bytes, New size: {new_size} bytes")
        print(f"Time taken to hide the message: {end_time - start_time} seconds")
    except Exception as e:
        print(f"An error occurred: {e}")

def reveal_message(image_path, generator):
    start_time = time.time()
    try:
        secret_message = lsb.reveal(image_path, generator)
        end_time = time.time()
        if secret_message is None:
            print("No secret message found.")
        else:
            print(f"Secret message: {secret_message}")
            print(f"Time taken to reveal the message: {end_time - start_time} seconds")
    except Exception as e:
        print(f"An error occurred: {e}")

def main():
    parser = argparse.ArgumentParser(description="Steganography: Hide or reveal a message in an image using a pseudorandom number generator.")
    parser.add_argument("action", choices=["hide", "reveal"])
    parser.add_argument("image_path")
    parser.add_argument("--output_path")
    parser.add_argument("--message")
    parser.add_argument("--generator", choices=["eratosthenes", "fibonacci"], required=True)

    args = parser.parse_args()

    if args.generator == "eratosthenes":
        generator = generators.eratosthenes()
    elif args.generator == "fibonacci":
        generator = generators.fibonacci()

    if args.action == "hide":
        if not args.output_path or not args.message:
            print("Both --output_path and --message are required for hiding a message.")
            return
        hide_message(args.image_path, args.output_path, args.message, generator)
    elif args.action == "reveal":
        reveal_message(args.image_path, generator)

if __name__ == "__main__":
    main()
