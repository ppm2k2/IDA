SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==================================================================================================
-- Author:		Simon Mazzucca
-- Create date: 02/04/16
-- Description:	gets value of field [Issuer_Counterparty] 
--              where [Prime Broker/Clearing Broker] = @primeBrokerClearingBroker
--
-- TODO:        3 more versions to return:
--              - Issuer_CounterParty_Affiliate
--              - Issuer_Counterparty_Affiliate_Other
--              - Issuer_Counterparty_LegalName
-- ==================================================================================================
ALTER FUNCTION fn_Lookup_Gruss_Counterparties_Issuer_Counterparty
(
	@primeBrokerClearingBroker VARCHAR(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN

	DECLARE @issuerCounterparty VARCHAR(MAX) = 
		(SELECT [Issuer_Counterparty] 
		FROM [DEMI]..[Gruss_Counterparties]
		WHERE [Prime Broker/Clearing Broker] = @primeBrokerClearingBroker)

	RETURN @issuerCounterparty

END
GO


SELECT DEMI.dbo.[fn_Lookup_Value_in_Gruss_Counterparties] ('JPM-ICE')