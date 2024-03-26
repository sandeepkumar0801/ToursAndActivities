namespace ServiceAdapters.WirecardPayment.Constant
{
    public sealed class Constants
    {
        public const string BookBackRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'><W_REQUEST><W_JOB>" +
                "<JobID>{0}</JobID>" +
                "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                "<FNC_CC_BOOKBACK> <FunctionID>Bookback  1</FunctionID>" +
                "<CC_TRANSACTION><TransactionID>{2}</TransactionID>" +
                "<GuWID>{3}</GuWID>" +
                "<Amount minorunits=\"2\" action=\"convert\">{4}</Amount>" +
                "</CC_TRANSACTION>" +
                "</FNC_CC_BOOKBACK>" +
                "</W_JOB>" +
                "</W_REQUEST>" +
                "</WIRECARD_BXML>";

        public const string CapturePreauthorizeRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                 "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                 "<W_REQUEST><W_JOB>" +
                 "<JobID>{0}</JobID>" +
                 "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                 "<FNC_CC_CAPTURE_PREAUTHORIZATION> <FunctionID>Caturing 1</FunctionID>" +
                 "<CC_TRANSACTION><TransactionID>{2}</TransactionID>" +
                 "<GuWID>{3}</GuWID>" +
                 "<Amount minorunits=\"2\" action=\"convert\">{4}</Amount>" +
                 "</CC_TRANSACTION>" +
                 "</FNC_CC_CAPTURE_PREAUTHORIZATION>" +
                 "</W_JOB>" +
                 "</W_REQUEST>" +
                 "</WIRECARD_BXML>";

        public const string ProcessPaymentRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                       "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                       "<W_REQUEST><W_JOB>" +
                                                       "<JobID>{0}</JobID>" +
                                                       "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                                                       "<{2}> <FunctionID>{3}</FunctionID>" +
                                                       "<CC_TRANSACTION><TransactionID>{4}</TransactionID>" +
                                                       "<Amount minorunits=\"2\" action=\"convert\">{5}</Amount>" +
                                                       "<Currency>{6}</Currency>" +
                                                       "<CountryCode>{7}</CountryCode>" +
                                                       "<RECURRING_TRANSACTION><Type>Single</Type></RECURRING_TRANSACTION>" +
                                                       "<CREDIT_CARD_DATA><CreditCardNumber>{8}</CreditCardNumber>" +
                                                       "<CVC2>{9}</CVC2>" +
                                                       "<ExpirationYear>{10}</ExpirationYear>" +
                                                       "<ExpirationMonth>{11}</ExpirationMonth>" +
                                                       "<CardHolderName>{12}</CardHolderName>" +
                                                       "</CREDIT_CARD_DATA>" +
                                                       "<CONTACT_DATA><IPAddress>{13}</IPAddress></CONTACT_DATA>" +
                                                       "<CORPTRUSTCENTER_DATA>" +
                                                       "<ADDRESS>" +
                                                       "<FirstName>{14}</FirstName>" +
                                                       "<LastName>{15}</LastName>" +
                                                       "<Address1>{16}</Address1>" +
                                                       "<City>{17}</City>" +
                                                       "<ZipCode>{18}</ZipCode>" +
                                                       "<State>{19}</State>" +
                                                       "<Country>{20}</Country>" +
                                                       "<Email>{21}</Email>" +
                                                       "</ADDRESS>" +
                                                       "</CORPTRUSTCENTER_DATA>" +
                                                       "</CC_TRANSACTION>" +
                                                       "</{22}>" +
                                                       "</W_JOB>" +
                                                       "</W_REQUEST>" +
                                                       "</WIRECARD_BXML>";

        public const string RequestXMlExceptCreditCard = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                       "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                       "<W_REQUEST><W_JOB>" +
                                                       "<JobID>{0}</JobID>" +
                                                       "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                                                       "<{2}> <FunctionID>{3}</FunctionID>" +
                                                       "<CC_TRANSACTION><TransactionID>{4}</TransactionID>" +
                                                       "<Amount minorunits=\"2\" action=\"convert\">{5}</Amount>" +
                                                       "<Currency>{6}</Currency>" +
                                                       "<CountryCode>{7}</CountryCode>" +
                                                       "<RECURRING_TRANSACTION><Type>Single</Type></RECURRING_TRANSACTION>" +
                                                       "<CREDIT_CARD_DATA><CreditCardNumber>{8}</CreditCardNumber>" +
                                                       "<CVC2>{9}</CVC2>" +
                                                       "<ExpirationYear>{10}</ExpirationYear>" +
                                                       "<ExpirationMonth>{11}</ExpirationMonth>" +
                                                       "<CardHolderName>{12}</CardHolderName>" +
                                                       "</CREDIT_CARD_DATA>" +
                                                       "<CORPTRUSTCENTER_DATA>" +
                                                       "<ADDRESS>" +
                                                       "<FirstName>{13}</FirstName>" +
                                                       "<LastName>{14}</LastName>" +
                                                       "<Address1>{15}</Address1>" +
                                                       "<City>{16}</City>" +
                                                       "<ZipCode>{17}</ZipCode>" +
                                                       "<State>{18}</State>" +
                                                       "<Country>{19}</Country>" +
                                                       "<Email>{20}</Email>" +
                                                       "</ADDRESS>" +
                                                       "</CORPTRUSTCENTER_DATA>" +
                                                       "</CC_TRANSACTION>" +
                                                       "</{21}>" +
                                                       "</W_JOB>" +
                                                       "</W_REQUEST>" +
                                                       "</WIRECARD_BXML>";

        public const string RollBackRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                 "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                 "<W_REQUEST><W_JOB>" +
                                                 "<JobID>{0}</JobID>" +
                                                 "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                                                 "<FNC_CC_REVERSAL> <FunctionID>Bookback 1</FunctionID>" +
                                                 "<CC_TRANSACTION><TransactionID>{2}</TransactionID>" +
                                                 "<GuWID>{3}</GuWID>" +
                                                 "</CC_TRANSACTION>" +
                                                 "</FNC_CC_REVERSAL>" +
                                                 "</W_JOB>" +
                                                 "</W_REQUEST>" +
                                                 "</WIRECARD_BXML>";

        public const string EmiEnrollmentCheckRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                           "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                           "<W_REQUEST><W_JOB>" +
                                                           "<JobID>JobId1</JobID>" +
                                                           "<BusinessCaseSignature>{0}</BusinessCaseSignature>" +
                                                           "<FNC_CC_ENROLLMENT_CHECK>" +
                                                           "<FunctionID>{1}</FunctionID>" +
                                                           "<CC_TRANSACTION><TransactionID>1</TransactionID>" +
                                                           "<Amount minorunits=\"2\" action=\"convert\">{2}</Amount>" +
                                                           "<Currency>{3}</Currency>" +
                                                           "<CountryCode>{4}</CountryCode>" +
                                                           "<Usage>papappala papp</Usage>" +
                                                           "<RECURRING_TRANSACTION><Type>Single</Type></RECURRING_TRANSACTION>" +
                                                           "<CREDIT_CARD_DATA><CreditCardNumber>{5}</CreditCardNumber>" +
                                                           "<CVC2>{6}</CVC2>" +
                                                           "<ExpirationYear>{7}</ExpirationYear>" +
                                                           "<ExpirationMonth>{8}</ExpirationMonth>" +
                                                           "<CardHolderName>{9}</CardHolderName>" +
                                                           "</CREDIT_CARD_DATA>" +
                                                           "<CONTACT_DATA>" +
                                                           "<IPAddress>{10}</IPAddress>" +
                                                           "<BROWSER>" +
                                                           "<AcceptHeader>{11}</AcceptHeader>" +
                                                           "<UserAgent>{12}</UserAgent>" +
                                                           "<DeviceCategory>{13}</DeviceCategory>" +
                                                           "</BROWSER>" +
                                                           "</CONTACT_DATA>" +
                                                           "<CORPTRUSTCENTER_DATA>" +
                                                           "<ADDRESS>" +
                                                           "<FirstName>{14}</FirstName>" +
                                                           "<LastName>{15}</LastName>" +
                                                           "<Address1>{16}</Address1>" +
                                                           "<City>{17}</City>" +
                                                           "<ZipCode>{18}</ZipCode>" +
                                                           "<State>{19}</State>" +
                                                           "<Country>{20}</Country>" +
                                                           "<Email>{21}</Email>" +
                                                           "</ADDRESS>" +
                                                           "</CORPTRUSTCENTER_DATA>" +
                                                           "</CC_TRANSACTION>" +
                                                           "</FNC_CC_ENROLLMENT_CHECK>" +
                                                           "</W_JOB>" +
                                                           "</W_REQUEST>" +
                                                           "</WIRECARD_BXML>";

        public const string CapturePreauthorize3DRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                              "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                              "<W_REQUEST><W_JOB>" +
                                                              "<JobID>{0}</JobID>" +
                                                              "<BusinessCaseSignature>{1}</BusinessCaseSignature>" +
                                                              "<FNC_CC_CAPTURING> <FunctionID>Caturing 1</FunctionID>" +
                                                              "<CC_TRANSACTION><TransactionID>{2}</TransactionID>" +
                                                              "<GuWID>{3}</GuWID>" +
                                                              "<Amount minorunits=\"2\" action=\"convert\">{4}</Amount>" +
                                                              "<AuthorizationCode>{5}</AuthorizationCode>" +
                                                              "<Currency>{6}</Currency>" +
                                                              "</CC_TRANSACTION>" +
                                                              "</FNC_CC_CAPTURING>" +
                                                              "</W_JOB>" +
                                                              "</W_REQUEST>" +
                                                              "</WIRECARD_BXML>";

        public const string EnrollmentCheckRequestXml = "<?xml version='1.0' encoding='UTF-8'?>" +
                                                           "<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>" +
                                                           "<W_REQUEST><W_JOB>" +
                                                           "<JobID>JobId1</JobID>" +
                                                           "<BusinessCaseSignature>{0}</BusinessCaseSignature>" +
                                                           "<FNC_CC_ENROLLMENT_CHECK>" +
                                                           "<FunctionID>{1}</FunctionID>" +
                                                           "<CC_TRANSACTION><TransactionID>1</TransactionID>" +
                                                           "<Amount minorunits=\"2\" action=\"convert\">{2}</Amount>" +
                                                           "<Currency>{3}</Currency>" +
                                                           "<CountryCode>{4}</CountryCode>" +
                                                           "<Usage>papappala papp</Usage>" +
                                                           "<RECURRING_TRANSACTION><Type>Single</Type></RECURRING_TRANSACTION>" +
                                                           "<CREDIT_CARD_DATA><CreditCardNumber>{5}</CreditCardNumber>" +
                                                           "<CVC2>{6}</CVC2>" +
                                                           "<ExpirationYear>{7}</ExpirationYear>" +
                                                           "<ExpirationMonth>{8}</ExpirationMonth>" +
                                                           "<CardHolderName>{9}</CardHolderName>" +
                                                           "</CREDIT_CARD_DATA>" +
                                                           "<CONTACT_DATA>" +
                                                           "<IPAddress>{10}</IPAddress>" +
                                                           "<BROWSER>" +
                                                           "<AcceptHeader>{11}</AcceptHeader>" +
                                                           "<UserAgent>{12}</UserAgent>" +
                                                           "<DeviceCategory>{13}</DeviceCategory>" +
                                                           "</BROWSER>" +
                                                           "</CONTACT_DATA>" +
                                                           "<CORPTRUSTCENTER_DATA>" +
                                                           "<ADDRESS>" +
                                                           "<FirstName>{14}</FirstName>" +
                                                           "<LastName>{15}</LastName>" +
                                                           "<Address1>{16}</Address1>" +
                                                           "<City>{17}</City>" +
                                                           "<ZipCode>{18}</ZipCode>" +
                                                           "<State>{19}</State>" +
                                                           "<Country>{20}</Country>" +
                                                           "<Email>{21}</Email>" +
                                                           "</ADDRESS>" +
                                                           "</CORPTRUSTCENTER_DATA>" +
                                                           "</CC_TRANSACTION>" +
                                                           "</FNC_CC_ENROLLMENT_CHECK>" +
                                                           "</W_JOB>" +
                                                           "</W_REQUEST>" +
                                                           "</WIRECARD_BXML>";

        public const string ErrorCode524 = "Error524";
        public const string ErrorCode523 = "Error523";
        public const string ErrorCode522 = "Error522";
        public const string ErrorCode539 = "Error539";
        public const string PreDefinedErrorCodes = "PreDefinedErrorCodes";
        public const string EnrollStatusNotEnrolled = "NotEnrolled";
        public const string EnrollStatusError = "Error";
        public const string EnrollStatusInEligible = "Ineligible";
        public const string EnrollStatusFailure = "Failure";
        public const string BookingGuidString = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/GuWID";
        public const string BookingStatusType = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/StatusType";
        public const string ResponseError = "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/ERROR";
    }
}