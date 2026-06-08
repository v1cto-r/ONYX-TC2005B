from database.database import get_db_connection

connection = get_db_connection()

def get_prompts_and_comments(user_id: int , search_text: str | None, category_id: int | None, department_id: int | None):
    cursor = connection.cursor(dictionary=True)

    cursor.callproc("GetPrompts", [user_id, search_text, category_id, department_id])
    prompts_result = next(cursor.stored_results())
    prompts = prompts_result.fetchall()

    if not prompts:
        cursor.close()
        return []

    prompt_ids = [p['promptId'] for p in prompts]

    format_strings = ','.join(['%s'] * len(prompt_ids))
    comments_query = f"""
            SELECT
                comment_id AS commentId,
                prompt_id AS commentPromptId,
                user_id AS commentUserId,
                comment AS commentContent
            FROM Comments
            WHERE prompt_id IN ({format_strings});
        """

    cursor.execute(comments_query, tuple(prompt_ids))
    comments = cursor.fetchall()

    cursor.close()

    comments_map = {}
    for c in comments:
        prompt_id = c.pop('commentPromptId')
        if prompt_id not in comments_map:
            comments_map[prompt_id] = []
        comments_map[prompt_id].append(c)

    for prompt in prompts:
        prompt['comments'] = comments_map.get(prompt['promptId'], [])
        if prompt['promptCreatedAt']:
            prompt['promptCreatedAt'] = prompt['promptCreatedAt'].isoformat()

    return prompts

def add_prompt(user_id: int, prompt_title: str, prompt_text: str, category_id: int, department_id: int):
    try:
        cursor = connection.cursor(dictionary=True)
        sql = "INSERT INTO Prompts (user_id, title, prompt, category_id, department_id) VALUES (%s, %s, %s, %s, %s)"
        values = (user_id, prompt_title, prompt_text, category_id, department_id)

        cursor.execute(sql, values)
        connection.commit()
        cursor.close()
        return True
    except:
        return False


def rate_prompt(prompt_id: int, user_id: int, rating: int):
    try:
        cursor = connection.cursor(dictionary=True)
        cursor.execute(
            "SELECT rating FROM rating WHERE user_id = %s AND prompt_id = %s",
            [user_id, prompt_id]
        )
        existing = cursor.fetchone()

        if existing is None:
            cursor.execute(
                "INSERT INTO rating (user_id, prompt_id, rating) VALUES (%s, %s, %s)",
                [user_id, prompt_id, rating]
            )
        elif existing['rating'] == rating:
            cursor.execute(
                "DELETE FROM rating WHERE user_id = %s AND prompt_id = %s",
                [user_id, prompt_id]
            )
        else:
            cursor.execute(
                "UPDATE rating SET rating = %s WHERE user_id = %s AND prompt_id = %s",
                [rating, user_id, prompt_id]
            )

        connection.commit()
        cursor.close()
        return True
    except:
        return False


def add_comment(prompt_id: int, user_id: int, comment: str):
    try:
        cursor = connection.cursor(dictionary=True)
        cursor.execute(
            "INSERT INTO comments (user_id, prompt_id, comment) VALUES (%s, %s, %s)",
            [user_id, prompt_id, comment]
        )
        connection.commit()
        cursor.close()
        return True
    except:
        return False


def toggle_save(prompt_id: int, user_id: int):
    try:
        cursor = connection.cursor(dictionary=True)
        cursor.execute(
            "SELECT 1 FROM saved WHERE user_id = %s AND prompt_id = %s",
            [user_id, prompt_id]
        )
        existing = cursor.fetchone()

        if existing:
            cursor.execute(
                "DELETE FROM saved WHERE user_id = %s AND prompt_id = %s",
                [user_id, prompt_id]
            )
            action = "unsaved"
        else:
            cursor.execute(
                "INSERT INTO saved (user_id, prompt_id) VALUES (%s, %s)",
                [user_id, prompt_id]
            )
            action = "saved"

        connection.commit()
        cursor.close()
        return action
    except:
        return None
