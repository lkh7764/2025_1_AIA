import torch
import torch.nn as nn
import torchvision
import torchvision.transforms as transforms
from torch.utils.data import DataLoader
from sklearn.metrics.pairwise import cosine_similarity
import numpy as np
from PIL import Image

class CNN(nn.Module):
    def __init__(self, feature_dim=128):
        super(CNN, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(3, 32, 3, padding=1), nn.ReLU(),
            nn.MaxPool2d(2),
            nn.Conv2d(32, 64, 3, padding=1), nn.ReLU(),
            nn.MaxPool2d(2),
            nn.Conv2d(64, 128, 3, padding=1), nn.ReLU(),
            nn.AdaptiveAvgPool2d((1, 1))
        )
        self.classifier = nn.Linear(128, 10)

    def forward(self, x):
        x = self.features(x).view(x.size(0), -1)
        x = self.classifier(x)
        return x

    def extract_feature(self, x):
        with torch.no_grad():
            feat = self.features(x).view(x.size(0), -1)
        return feat

def load_cifar_dataset(root='./data', batch_size=64):
    transform = transforms.Compose([
        transforms.ToTensor(),
        transforms.Normalize((0.5, 0.5, 0.5), (0.5, 0.5, 0.5))
    ])
    dataset = torchvision.datasets.CIFAR10(root=root, train=True, download=True, transform=transform)
    loader = DataLoader(dataset, batch_size=batch_size, shuffle=False)
    return dataset, loader

def calculate_similarity(model, device, user_img_path, class_name, dataset):
    preprocess = transforms.Compose([
        transforms.Resize((32, 32)),
        transforms.ToTensor(),
        transforms.Normalize((0.5, 0.5, 0.5), (0.5, 0.5, 0.5))
    ])
    img = Image.open(user_img_path).convert('RGB')
    tensor = preprocess(img).unsqueeze(0).to(device)
    user_feat = model.extract_feature(tensor).cpu().numpy()

    class_index = dataset.classes.index(class_name)
    class_feats = []
    for img_tensor, label in dataset:
        if label == class_index:
            feat = model.extract_feature(img_tensor.unsqueeze(0).to(device)).cpu().numpy()
            class_feats.append(feat.flatten())
    class_feats = np.vstack(class_feats)

    sims = cosine_similarity(user_feat, class_feats)[0]
    return float(np.mean(sims))

def main():
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model = CNN().to(device)
    model.eval()

    dataset, _ = load_cifar_dataset()
    print("Loaded CIFAR-10 dataset with classes:", dataset.classes)

    user_img = input("Path to your drawing image: ").strip()
    print("Choose a target class from:", dataset.classes)
    cls = input("Target class: ").strip()
    score = calculate_similarity(model, device, user_img, cls, dataset)
    print(f"Average similarity to class '{cls}': {score:.4f}")

if __name__ == '__main__':
    main()
