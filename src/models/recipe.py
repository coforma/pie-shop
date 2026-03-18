from datetime import datetime
from typing import Optional, List
from pydantic import BaseModel


class PieRecipe(BaseModel):
    id: str
    name: str
    description: str
    baking_time: int
    baking_temp: int
    ingredients: List[dict]
    prep_steps: List[str]
    difficulty: str


PIE_CATALOG = {
    "apple": {
        "_id": "apple",
        "name": "Classic Apple Pie",
        "description": "Traditional American apple pie with cinnamon",
        "bakingTime": 45,
        "bakingTemp": 375,
        "ingredients": [
            {"item": "apples", "quantity": 6, "unit": "whole"},
            {"item": "sugar", "quantity": 0.75, "unit": "cup"},
            {"item": "cinnamon", "quantity": 1, "unit": "tsp"},
            {"item": "butter", "quantity": 2, "unit": "tbsp"},
            {"item": "flour", "quantity": 2.5, "unit": "cup"},
            {"item": "salt", "quantity": 1, "unit": "tsp"},
        ],
        "prepSteps": ["wash_fruit", "peel_fruit", "slice_fruit", "make_dough", "assemble"],
        "difficulty": "medium",
    },
    "cherry": {
        "_id": "cherry",
        "name": "Sweet Cherry Pie",
        "description": "Classic cherry pie with lattice crust",
        "bakingTime": 50,
        "bakingTemp": 400,
        "ingredients": [
            {"item": "cherries", "quantity": 4, "unit": "cup"},
            {"item": "sugar", "quantity": 1, "unit": "cup"},
            {"item": "cornstarch", "quantity": 3, "unit": "tbsp"},
            {"item": "flour", "quantity": 2.5, "unit": "cup"},
            {"item": "butter", "quantity": 0.5, "unit": "cup"},
        ],
        "prepSteps": ["wash_fruit", "pit_fruit", "make_dough", "assemble"],
        "difficulty": "medium",
    },
    "pumpkin": {
        "_id": "pumpkin",
        "name": "Spiced Pumpkin Pie",
        "description": "Creamy pumpkin custard in a buttery crust",
        "bakingTime": 55,
        "bakingTemp": 350,
        "ingredients": [
            {"item": "pumpkin_puree", "quantity": 15, "unit": "oz"},
            {"item": "eggs", "quantity": 3, "unit": "whole"},
            {"item": "heavy_cream", "quantity": 1, "unit": "cup"},
            {"item": "sugar", "quantity": 0.75, "unit": "cup"},
            {"item": "pumpkin_spice", "quantity": 2, "unit": "tsp"},
        ],
        "prepSteps": ["make_dough", "mix_filling", "assemble"],
        "difficulty": "easy",
    },
    "pecan": {
        "_id": "pecan",
        "name": "Southern Pecan Pie",
        "description": "Rich, sweet pecan pie with a gooey filling",
        "bakingTime": 60,
        "bakingTemp": 350,
        "ingredients": [
            {"item": "pecans", "quantity": 2, "unit": "cup"},
            {"item": "corn_syrup", "quantity": 1, "unit": "cup"},
            {"item": "eggs", "quantity": 3, "unit": "whole"},
            {"item": "butter", "quantity": 2, "unit": "tbsp"},
            {"item": "vanilla", "quantity": 1, "unit": "tsp"},
            {"item": "flour", "quantity": 2.5, "unit": "cup"},
        ],
        "prepSteps": ["toast_nuts", "make_dough", "mix_filling", "assemble"],
        "difficulty": "easy",
    },
    "blueberry": {
        "_id": "blueberry",
        "name": "Blueberry Burst Pie",
        "description": "Fresh blueberry pie with a crumb topping",
        "bakingTime": 45,
        "bakingTemp": 375,
        "ingredients": [
            {"item": "blueberries", "quantity": 5, "unit": "cup"},
            {"item": "sugar", "quantity": 0.75, "unit": "cup"},
            {"item": "lemon_juice", "quantity": 2, "unit": "tbsp"},
            {"item": "flour", "quantity": 2.5, "unit": "cup"},
            {"item": "butter", "quantity": 0.5, "unit": "cup"},
        ],
        "prepSteps": ["wash_fruit", "make_dough", "assemble"],
        "difficulty": "easy",
    },
}
