import torch



from Models.cnn_model import CNN

def load_subject_model(model_path: str, device=None): 
    if device is None:
        device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

    model = CNN()
    state_dict = torch.load(model_path, map_location=device)
    model.load_state_dict(state_dict)
    model.to(device)
    model.eval()

    return model, device


MODEL_PATH = "Models/subject_cnn.pth"
subject_model, DEVICE = load_subject_model(MODEL_PATH)


import numpy as np
from PIL import Image
import torchvision.transforms as transforms
from sklearn.metrics.pairwise import cosine_similarity

preprocess = transforms.Compose([
    transforms.Resize((64,64)),
    transforms.ToTensor(),
    transforms.Normalize(   (0.5, 0.5, 0.5),
                            (0.5, 0.5, 0.5))
])

def subject_similarity(image_path, target_subject):
    return 