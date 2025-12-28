from Services.Subject import subject_similarity
from Services.Preference import analyze_preferences
from Services.Scoring import aggregate_score
from Services.LLM import generate_feedback


def evaluate_picture_internal(payload):
    image_path = payload["image_path"]
    buyer_id = payload["buyer_id"]
    request_info = payload["request"]

    subject_result = subject_similarity(
        image_path = image_path,
        target_subject = request_info["subject"]
    )

    pref_result = analyze_preferences(
        image_path = image_path,
        buyer_id = buyer_id
    )

    final_score = aggregate_score(
        subject_score = subject_result["score"],
        pref_score = pref_result["score"],
        weights = request_info.get("weights")
    )

    feedback = generate_feedback(
        buyer_id = buyer_id,
        subject_result = subject_result,
        pref_result = pref_result,
        final_score = final_score
    )

    return {
        "subject": subject_result,
        "pref": pref_result,
        "final": final_score,
        "feedback": feedback
    }
