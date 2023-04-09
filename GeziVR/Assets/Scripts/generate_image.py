import openai
import requests

def generate_images():

    openai.api_key = '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'
    idea_for_image = generate_idea_for_image()

    images = openai.Image.create(
        prompt=idea_for_image,
        n=8,
        size="1024x1024"
    )
    k = 1
    for image in images['data']:
        save_image("../GeneratedImages/img{}.png".format(k), image['url'])
        k += 1

def generate_idea_for_image():
    openai.api_key = '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'
    resp = openai.ChatCompletion.create(
        model="gpt-3.5-turbo",
        messages=[
            {"role": "user", "content": "Random idea for a picture, start the sentence with 'Create'"}
        ]
    )
    return resp['choices'][0]['message']['content']

def save_image(file_path, url):
    img_data = requests.get(url).content
    with open(file_path, 'wb') as handler:
        handler.write(img_data)


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    generate_images()
# See PyCharm help at https://www.jetbrains.com/help/pycharm/
