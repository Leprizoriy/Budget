using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Extensions
{
    public static class StringExtensions
    {
        public static string TranslateErrorMessageToRU(this string message)
        {
            string errorMessage = string.Empty;

            switch (message)
            {
                case "Sign-In Failed. Errors [Password incorrect!]":
                    errorMessage = "Не получилось войти в систему. Неверный пароль.";
                    break;
                case "Empty user name":
                    errorMessage = "Не получилось войти в систему. Имя пользователя обязательное поле.";
                    break;
                case "Empty password":
                    errorMessage = "Не получилось войти в систему. Пароль обязательное поле.";
                    break;
                case "Empty email":
                    errorMessage = "Не получилось войти в систему. Почта обязательное поле.";
                    break;
                case "Empty first name":
                    errorMessage = "Не получилось войти в систему. Имя обязательное поле.";
                    break;
                case "Empty last name":
                    errorMessage = "Не получилось войти в систему. Фамилия обязательное поле.";
                    break;
                case "Sign-Up Failed. Errors [User already exists!]":
                    errorMessage = "Не получилось войти в систему. Такой пользователь уже существует.";
                    break;
                case "Empty name":
                    errorMessage = "Название обязательное поле.";
                    break;
                case "Empty currency":
                    errorMessage = "Тип валюты не выбран.";
                    break;
                case "Select a category":
                    errorMessage = "Категория не выбрана.";
                    break;
                case "Select a second category":
                    errorMessage = "Вторая категория не выбрана.";
                    break;
                case "Percentage must be greater than 0":
                    errorMessage = "Процент должен быть больше 0.";
                    break;
                case "Percentage must be less than 100":
                    errorMessage = "Процент должен быть меньше 100.";
                    break;
                case "The sum of percentages in categories cannot be more than 100":
                    errorMessage = "Сумма процентов в категориях не может быть больше 100.";
                    break;
                case "There is already a percentage for this category":
                    errorMessage = "Для этой категории уже есть процент.";
                    break;
                case "Amount must be greater than 0":
                    errorMessage = "Сумма должна быть больше 0.";
                    break;
                case "Not all the money has been distributed":
                    errorMessage = "Не вся сумма распределена.";
                    break;
                case "No categories for this currency":
                    errorMessage = "Нет категорий для выбранного типа валют.";
                    break;
                case "This category already exists":
                    errorMessage = "Такая категория уже существует.";
                    break;
                case "Can't delete category. This category has a subcategory":
                    errorMessage = "Невозможно удалить категорию. У данной категории есть подкатегория.";
                    break;
                case "This category has incomes or outgoings. Are you sure you want to delete this category?":
                    errorMessage = "У данной категории есть накопления или долг. Вы уверенны что хотите удалить категорию?";
                    break;
                case "Transaction was not selected":
                    errorMessage = "Транзакция не была выбрана.";
                    break;
                case "Category setting was not selected":
                    errorMessage = "Настройка категории не была выбрана.";
                    break;
                case "Please use a dot instead of a comma":
                    errorMessage = "Пожалуйста используйте точку вместо запятой.";
                    break;
                case "You cannot exchange savings for the same category":
                    errorMessage = "Нельзя сделать обмен накоплений для одной и той же категории.";
                    break;
                case "You cannot exchange savings for the same subcategory":
                    errorMessage = "Нельзя сделать обмен накоплений для одной и той же подкатегории.";
                    break;
                default:
                    break;
            }

            return errorMessage;
        }
    }
}
