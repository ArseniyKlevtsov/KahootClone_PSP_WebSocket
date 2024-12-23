﻿using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public class AddQuestionToRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<AddQuestionToRoomDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);
        if (room == null)
        {
            throw new ArgumentException("Room with ID {client.ID} not found");
        }

        if (room.CreatorId != dto.CreatorId)
        {
            throw new ArgumentException($"Access denied");
        }

        var question = new KahootQuestion(dto.Question, dto.CorrectAnswerIndex, dto.Answers);
        room.Questions.Add(question);

        response = new Message("QuestionCreated", question);
        return new HandlerResponse(response);
    }
}
