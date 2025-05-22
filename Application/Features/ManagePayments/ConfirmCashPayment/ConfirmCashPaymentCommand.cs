using System;
using Application.DTOs.Payment;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.ConfirmCashPayment;

public class ConfirmCashPaymentCommand : IRequest<Result>
{
    public Guid PaymentId { get; set; }
    public decimal AmountPaid { get; set; }
}

