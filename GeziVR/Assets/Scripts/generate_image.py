import openai
import requests

def generate_images():
    openai.api_key = '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'
    images = openai.Image.create(
        prompt="random images",
        n=1,
        size="1024x1024"
    )
    for image in images['data']:
        save_image("deneme.jpeg", image['url'])

def save_image(file_path, url):
    img_data = requests.get(url).content
    with open(file_path, 'wb') as handler:
        handler.write(img_data)


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    generate_images()
# See PyCharm help at https://www.jetbrains.com/help/pycharm/
