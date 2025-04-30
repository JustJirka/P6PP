using Analytics.Application.DTOs;
using Analytics.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.Controllers
{
    namespace PaymentService.API.Controllers
    {
        [ApiController] // Indicates a Web API controller :contentReference[oaicite:0]{index=0}
        [Route("api/[controller]")]
        public class PaymentsController(
            IPaymentsService paymentService,
            IDatabaseSyncService databaseSyncService,
            ILogger<PaymentsController> logger) : ControllerBase
        {
            // GET: api/Payments
            [HttpGet]
            public async Task<ActionResult<List<PaymentDto>>> GetAllPayments()
            {
                try
                {
                    var payments = await paymentService.GetAllPayments(); // returns List<Payment> :contentReference[oaicite:1]{index=1}

                    logger.LogInformation($"Retrieved {payments.Count} payments");

                    // Map domain model → DTO
                    var dtos = payments.Select(p => new PaymentDto
                    {
                        id = p.Id,
                        userId = p.UserId,
                        roleId = p.RoleId,
                        price = p.Price,
                        creditAmount = p.CreditAmount,
                        status = p.Status,
                        transactionType = p.TransactionType,
                        createdAt = p.CreatedAt.ToString("o") // ISO-8601 format :contentReference[oaicite:2]{index=2}
                    }).ToList();

                    return Ok(dtos);
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, "Error retrieving payments");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: api/Payments/{paymentId}
            [HttpGet("{id:int}")]
            public async Task<ActionResult<PaymentDto>> GetPaymentById(int id)
            {
                try
                {
                    var payment = await paymentService.GetPaymentById(id);
                    if (payment == null)
                        return NotFound();

                    var dto = new PaymentDto
                    {
                        id = payment.Id,
                        userId = payment.UserId,
                        roleId = payment.RoleId,
                        price = payment.Price,
                        creditAmount = payment.CreditAmount,
                        status = payment.Status,
                        transactionType = payment.TransactionType,
                        createdAt = payment.CreatedAt.ToString("o")
                    };

                    return Ok(dto);
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, $"Error retrieving payment with ID {id}");
                    return StatusCode(500, "Internal server error");
                }
            }

            // POST: api/Payments
            [HttpPost]
            public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] PaymentDto paymentDto)
            {
                try
                {
                    logger.LogInformation("Received payment: {@Payment}", paymentDto);

                    if (paymentDto == null)
                        return BadRequest("Payment data is missing.");

                    // Optionally validate required fields
                    if (string.IsNullOrEmpty(paymentDto.createdAt))
                        return BadRequest("createdAt is required.");

                    var created = await paymentService.CreatePayment(paymentDto);

                    var dto = new PaymentDto
                    {
                        id = created.Id,
                        userId = created.UserId,
                        roleId = created.RoleId,
                        price = created.Price,
                        creditAmount = created.CreditAmount,
                        status = created.Status,
                        transactionType = created.TransactionType,
                        createdAt = created.CreatedAt.ToString("o")
                    };

                    // Returns 201 with Location header :contentReference[oaicite:3]{index=3}
                    return CreatedAtAction(nameof(GetPaymentById), new { id = dto.id }, dto);
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, "Error creating payment");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            // DELETE: api/Payments/{paymentId}
            [HttpDelete("{id:int}")]
            public async Task<ActionResult<PaymentDto>> DeletePayment(int id)
            {
                try
                {
                    var deleted = await paymentService.DeletePayment(id);
                    if (deleted == null)
                        return NotFound();

                    var dto = new PaymentDto
                    {
                        id = deleted.Id,
                        userId = deleted.UserId,
                        roleId = deleted.RoleId,
                        price = deleted.Price,
                        creditAmount = deleted.CreditAmount,
                        status = deleted.Status,
                        transactionType = deleted.TransactionType,
                        createdAt = deleted.CreatedAt.ToString("o")
                    };

                    return Ok(dto);
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, $"Error deleting payment with ID {id}");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: api/Payments/triggerSync
            [HttpGet("triggerSync")]
            public async Task<IActionResult> TriggerSync()
            {
                try
                {
                    logger.LogInformation("Payment sync triggered.");
                    await databaseSyncService.SyncDatabase();
                    return Ok("Sync triggered successfully.");
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, "Error triggering payment database sync");
                    return StatusCode(500, "Internal server error");
                }
            }
        }
    }
}