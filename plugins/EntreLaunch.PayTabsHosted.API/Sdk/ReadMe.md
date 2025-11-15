https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/60000333779
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions

PT2 API Endpoints | Request Parameters
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/folders/60000492856


PT2 API Endpoints | Response Parameters
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/folders/60000492863



3.2 PT2 API Endpoints | Initiating the Payment Request
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000818182-3-2-pt2-api-endpoints-initiating-the-payment-request

Step 3 - PT2 API Endpoints | Initiating the payment
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000818168-step-3-pt2-api-endpoints-initiating-the-payment


sss Step 4 - Hosted Payment Page APIs | Accepting the payment
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000978874-step-4-hosted-payment-page-apis-accepting-the-payment
1- Validating the Request
2- Checking Request Applicability


What is the "tran_type" (transaction type)?
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000711310-what-is-the-tran-type-transaction-type-

//profile_id   indicates the merchant Profile ID you can get from your PayTavs dashboard. 
"cart_id" is one of the mandatory parameters that the request should have, which indicates the cart/order/invoice/receipt id at the merchant end to easily relate the transaction to. 
"cart_description" is one of the mandatory parameters that the request should have, which Indicates the cart/order description at the merchant end to easily relate the transaction. 
"cart_currency" is one of the mandatory parameters that the request should have, which Indicates the transaction currency, which will the customer be charged with. 

"paypage_lang" is one of the mandatory parameters that the request should have, which indicates the payment page language that will be displayed to the customers. 
Only “en” or “ar” is sEntreLaunchported

The Customer Details also called Billing information is the customer/cardholder details that indicate the customer details for this payment. 
The Shipping Details indicate the shipping details for this payment. If provided, the payment page will be prefilled with the provided data. 
"card_discounts" is one of the optional parameters that the request should have, which provides discounts for some of your valued customers via the property card_discounts, which contains an array of objects each object for a specific range of customers cards discounts.

Direct Payment
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000818176-request-parameters-token-token-
https://sEntreLaunchport.paytabs.com/en/sEntreLaunchport/solutions/articles/60000805341-request-response-parameters-the-callback-url-callback-

TransactionReference

Transaction Reference (tran_ref) is the parameter that Indicates the Transaction Reference on PayTabs side.
Note that, this is the reference to all PayTabs transactions, also this can be used between the merchant and customer communications or between merchant and PayTabs team communications.


