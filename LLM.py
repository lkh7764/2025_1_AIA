import requests


"""
# check requests lib
response = requests.get("http://www.google.com")
print(response.status_code)
"""

def getRequestFromDeepSeek(theme):
    prompt = f"""
    
    {theme}이라는 주제의 그림을 의뢰하는 사람의 의뢰서 문장을 작성해주세요.
    주제는 직접적으로 말하지 않고, 힌트나 상태로 돌려 말합니다. 
    의뢰자의 이름도 힌트입니다.
    
    다음 예시를 참고하여 {theme}이라는 주제에 맞는 의뢰서 문장을 한국어로 작성해서 json 포맷으로 변환해주세요.
    
    예시1) 
    key1-'theme': cheese
    key2-'request': 너무 배가 고파요... 노랗고 구멍 뽕뽕 뚫린 걸 보면 기분이 좋아진답니다!
    key3-'client': 쮝쮝대마왕
    
    예시2)
    key1-'theme': fruit
    key2-'request': 요즘은 비타민이 부족해서인지 자꾸 피곤하네요. 상큼한 걸 보면 기운이 날 것 같아요!
    key3-'client': 아침헬창
    
    예시3)
    key1-'theme': cat
    key2-'request': 고향을 떠나온 지 어느덧 10년... 나의 친구가 보고싶다옹.
    key3-'client': 냥백작 37세
        
    예시4)
    key1-'theme': pizza
    key2-'request': 빵 위에 흩뿌려진 치즈처럼, 멈출 수 없는 짭짤한 유혹을 담아주시면 좋겠습니다.
    key3-'client': 페퍼로니
    
    예시5)
    key1-'theme': dinosaur
    key2-'request': 먼 옛날, 땅을 흔드는 거대한 발자국 소리가 들려요. 그 강력한 존재를 생생히 그려주실 수 있나요?
    key3-'client': 쿵쿵발자국장군
    
    예시6)
    key1-주제: coffee
    key2-'request': 따뜻한 원두 향이 코끝을 스치는 이 순간, 한 모금에 담긴 온기를 그림으로 전해주세요.
    key3-'client': 카페인러버
    
    예시7)
    key1-'theme': lemon
    key2-'request': 톡 터지는 새콤함에 정신이 번쩍 들 것 같아요. 노란 껍질 속 상큼함을 그대로 그려주세요!
    key3-'client': 새콤

    예시8)
    key1-'theme': tree
    key2-'request': 굵은 줄기와 울창한 가지들이 자라나는 힘을 담아주세요. 그 푸르른 생명력을 느낄 수 있게요.
    key3-'client': 숲속수호자
    
    output 양식) "theme": {theme}, "request": , "client":
    """
    
    res = requests.post(
        "http://localhost:11436/api/generate",
        json={
            "model": "gemma3:latest",
            "prompt": prompt,
            "stream": False
        }
    )
    
    # 2) JSON으로 파싱
    data = res.json()
    print(data['response'])
    
    return data['response']