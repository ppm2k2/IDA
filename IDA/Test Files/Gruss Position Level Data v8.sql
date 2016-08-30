DECLARE
	@sDate Datetime,
	@eDate Datetime

SET @sDate = '1/1/2015'
SET @eDate = '3/31/2015';


--DELETE FROM [FormPF].[dbo].[Gruss_Position_Level_Data] WHERE [Business_Date] BETWEEN @sDate AND @eDate;


WITH

	Underlying_Industry_CTE AS 
		(
		SELECT DISTINCT
			 CONVERT(DateTime,[Period End Date],101) AS 'Period End Date'
			,[Symbol]
			,CAST([Citco Security ID] AS [nvarchar](255)) AS 'Citco Security ID'
			,[Industry]
		FROM [FormPF_Input].[dbo].[Gruss_StdPos_ME]
		WHERE [Period End Date] IS NOT NULL
			AND [Industry] IS NOT NULL
			AND CONVERT(DateTime,[Period End Date],101) BETWEEN @sDate AND @eDate
		)
	
	
	,Position_Level_CTE AS
		(
		SELECT 
			'NULL' AS [ID]
			,'GRUSS' AS [PFA_Unique_Identifier]
			,CASE
				WHEN C.[Fund] = '>ARB' THEN 'GA'
				WHEN C.[Fund] = '>GGIE_MF' THEN 'GGIE'
				WHEN C.[Fund] = '>GGI_MF' THEN 'GGI'
				ELSE NULL
				END AS [Fund_Unique_Identifier]
			,CONVERT(Date,C.[Period End Date],101) AS [Business_Date]
			,CONVERT(Date,C.[Knowledge Date],101) AS [Information_Date]
			,C.[Fund] + '\' +
				C.[Period End Date] + '\' +
				COALESCE(C.[Trader],'NA') + '\' +
				COALESCE(C.[Standard Strategy],'NA') + '\' +
				COALESCE(C.[Strategy],'NA') + '\' +
				C.[Prime Broker/Clearing Broker] + '\' +
				COALESCE(C.[Citco Security ID],C.[Symbol],'NA') + '\' +
				COALESCE(C.[Repo],'NA')  + '\' +
				COALESCE(CAST(C.[Quantity(End)] AS varchar(255)),'NA') AS [Position_Unique_Identifier]
			
			
			,CASE 
			 WHEN C.[Repo] IS NOT NULL THEN 'Repos'
			 WHEN C.[Strategy] IN ('STUBS' /*, 'CQO AU'*/) THEN 'Investments in other sub-asset classes'
			 WHEN CP.[Issuer_Counterparty] = 'Lehman Claim' THEN 'Investments in other sub-asset classes'
			 WHEN C.[Security Asset Name] IN ('Single Stock Future') THEN 'Equity derivatives'
			 WHEN C.[Security Asset Name] IN ('Bank Debt') THEN 'Debt securities'
			 WHEN C.[Security Asset Name] IN ('CMO') THEN 'ABS/Structured products'
			 WHEN C.[Security Asset Name] IN ('Commodity Swaption') THEN 'Commodity derivatives'
			 WHEN C.[Security Asset Name] IN ('Currency Future Option') THEN 'FX derivatives'
			 WHEN C.[Security Asset Class] IN ('Bond') THEN
				CASE 
				WHEN C.[Security Asset Name] IN ('Credit Default Swap') THEN 'Credit derivatives'
				WHEN C.[Security Asset Name] IN ('MBS') THEN 'ABS/Structured Products'
				WHEN C.[Repo] IS NOT NULL THEN 'Repos'
				WHEN C.[Prime Broker/Clearing Broker] IN ('BONY') THEN 'Investments in funds for cash management'
				WHEN C.[Trader] IN ('TPOT') THEN 'Investments in funds for cash management'
				WHEN C.[Security Asset Name] IN ('Corporate Bond', 'Convertible Bond', 'Government Bond') THEN 'Debt Securities'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Currency') THEN 
				CASE WHEN C.[Security Asset Name] IN ('ACCOUNTING CASH') THEN
					CASE WHEN C.[Issue Ccy] IN ('USD') THEN 'Cash and cash equivalents'
					ELSE 'Non-US currency holdings'
					END
				WHEN C.[Security Asset Name] IN ('Currency') THEN'FX derivatives'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Equity') THEN
				CASE WHEN C.[Security Asset Name] IN ('Equity','Private Placement') THEN 'Equity securities'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Future') THEN
				CASE WHEN C.[Security Asset Name] IN ('Commodity Future') THEN 'Commodity derivatives'
				WHEN C.[Security Asset Name] IN ('Currency Future') THEN 'FX derivatives'
				WHEN C.[Security Asset Name] IN ('Index Future') THEN 'Equity derivatives'
				WHEN C.[Security Asset Name] IN ('IR Future','IR Future - Short Term') THEN 'Interest rate derivatives'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Option') THEN
				CASE WHEN C.[Security Asset Name] IN ('Equity Option', 'Index Option','Index Future Option') THEN 'Equity derivatives'
				WHEN C.[Security Asset Name] IN ('IR Future Option', 'IR Future Option - Short Term', 'Swaption (Interest Rate)') THEN 'Interest rate derivatives'
				WHEN C.[Security Asset Name] IN ('OTC FX Option') THEN 'FX derivatives'
				WHEN C.[Security Asset Name] IN ('OTC Bond Option') THEN 'Other derivatives'
				WHEN C.[Security Asset Name] IN ('Commodity Future Option') THEN 'Commodity derivatives'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Swap') THEN
				CASE WHEN C.[Security Asset Name] IN ('Equity Swap','Index Swap') THEN 'Equity derivatives'
				WHEN C.[Security Asset Name] IN ('IR/Currency Swap') THEN 'Interest rate derivatives'
				ELSE NULL
				END
			 ELSE NULL
			 END AS [Instrument_Group]



			,CASE 
			 WHEN C.[Repo] IS NOT NULL THEN 'Repos'
			 WHEN C.[Strategy] IN ('STUBS' /*, 'CQO AU'*/) THEN 'Investments in other sub-asset classes'
			 WHEN CP.[Issuer_Counterparty] = 'Lehman Claim' THEN 'Investments in other sub-asset classes'
			 WHEN C.[Security Asset Name] IN ('Single Stock Future') THEN 'Derivatives of listed equity'
			 WHEN C.[Security Asset Name] IN ('Bank Debt') THEN 'Leveraged loans'
			 WHEN C.[Security Asset Name] IN ('CMO') THEN 'Other ABS'
			 WHEN C.[Security Asset Name] IN ('Commodity Swaption') THEN 'Other commodities derivatives'
			 WHEN C.[Security Asset Name] IN ('Currency Future Option') THEN 'FX derivatives (investment)'
			 WHEN C.[Security Asset Class] IN ('Bond') THEN
				CASE 
				WHEN C.[Repo] IS NOT NULL THEN 'Repos'
				WHEN C.[Security Asset Name] IN ('MBS') THEN 'MBS'
				WHEN C.[Security Asset Name] IN ('Credit Default Swap') THEN
					CASE 
						WHEN C.[Security Description] LIKE 'CDX.%' THEN 'Index CDS'
						WHEN C.[Security Description] LIKE 'ITRX%' THEN 'Index CDS'
						ELSE 'Single name CDS'
						END
				WHEN C.[Prime Broker/Clearing Broker] IN ('BONY') THEN 'Investments in funds for cash management (non-MM funds)'
				WHEN C.[Trader] IN ('TPOT') THEN 'Investments in funds for cash management (non-MM funds)'
				WHEN C.[Security Asset Name] IN ('Corporate Bond') THEN 'Corporate non-convertible bonds'
				WHEN C.[Security Asset Name] IN ('Convertible Bond') THEN 'Convertible bonds'
				WHEN C.[Security Asset Name] IN ('Government Bond') THEN
					CASE WHEN C.[Security Description] LIKE 'FNMA%' THEN 'GSE bonds'
					WHEN C.[Security Description] LIKE 'FHLMC%' THEN 'GSE bonds'
					WHEN C.[Security Description] LIKE 'FHLB%' THEN 'GSE bonds'
					WHEN C.[Security Description] LIKE 'GNMA%' THEN 'Agency securities'
					WHEN C.[Security Description] LIKE 'T %' THEN 'U.S. Treasury securities'
					WHEN C.[Security Description] LIKE 'B %' THEN 'U.S. Treasury securities'
					WHEN C.[Security Description] LIKE 'WIT %' THEN 'U.S. Treasury securities'
					WHEN C.[Security Description] LIKE 'WI %' THEN 'U.S. Treasury securities'
					WHEN C.[Security Description] LIKE 'DO NOT USE*****T %' THEN 'U.S. Treasury securities'
					WHEN C.[Security Description] LIKE 'DO NOT USE******T %' THEN 'U.S. Treasury securities'
					WHEN R.[G10_NonUS] = 'Yes' THEN 'G10 non-US sovereign bonds'
					WHEN R.[G10_NonUS] = 'No' THEN 'Other sovereign and supranational bonds'	
					ELSE NULL 
					END
				ELSE NULL
				END
			WHEN C.[Security Asset Class] IN ('Currency') THEN 
				CASE WHEN C.[Security Asset Name] IN ('ACCOUNTING CASH') THEN
					CASE WHEN C.[Issue Ccy] IN ('USD') THEN 'Other cash and cash equivalents(excl. government securities)'
					ELSE 'Non-US currency holdings'
					END
				WHEN C.[Security Asset Name] IN ('Currency') THEN'FX derivatives (hedging)'
				ELSE NULL
				END
			WHEN C.[Security Asset Class] IN ('Equity') THEN
				CASE WHEN C.[Security Asset Name] IN ('Equity') THEN
					CASE WHEN C.[Citco Security ID] = '500001664' THEN 'Unlisted equity'
					WHEN C.[Strategy] = 'STUBS' THEN 'Unlisted equity'
					WHEN C.[Exchange Symbol] = 'OTC' THEN 'Unlisted equity'
					WHEN C.[Exchange Symbol] = 'PNK' THEN 'Unlisted equity'
					WHEN C.[Exchange Symbol] = 'OBB' THEN 'Unlisted equity'
					ELSE 'Listed equity'
					END
				WHEN C.[Security Asset Name] IN ('Private Placement') THEN 'Unlisted equity'
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Future') THEN
				CASE WHEN C.[Security Asset Name] IN ('Commodity Future') THEN
					CASE WHEN C.[Underlying Symbol] = 'CL' THEN 'Crude oil derivatives'
					WHEN C.[Underlying Symbol] = 'LCO' THEN 'Crude oil derivatives'
					WHEN C.[Underlying Symbol] = 'GC' THEN 'Gold derivatives'
					WHEN C.[Underlying Symbol] = 'NG' THEN 'Natural Gas derivatives'
		--			WHEN C.[Underlying Symbol] = '???' THEN 'Power derivatives'
					WHEN C.[Underlying Symbol] IN ('C','HG','HO','SB','SI','W','XB','2RB','3C') THEN 'Other commodities derivatives'
					ELSE NULL
					END
				WHEN C.[Security Asset Name] IN ('Currency Future') THEN 'FX derivatives (hedging)'
				WHEN C.[Security Asset Name] IN ('Index Future') THEN 'Derivatives of listed equity'
				WHEN C.[Security Asset Name] IN ('IR Future','IR Future - Short Term') THEN 'Interest rate derivatives'
				ELSE NULL
				END 
			 WHEN C.[Security Asset Class] IN ('Option') THEN
				CASE WHEN C.[Security Asset Name] IN ('Equity Option','Index Option','Index Future Option') THEN 'Derivatives of listed equity'
				WHEN C.[Security Asset Name] IN ('IR Future Option', 'IR Future Option - Short Term', 'Swaption (Interest Rate)') THEN 'Interest rate derivatives'
				WHEN C.[Security Asset Name] IN ('OTC FX Option') THEN 'FX derivatives (investment)'
				WHEN C.[Security Asset Name] IN ('OTC Bond Option') THEN 'Other derivatives'
				WHEN C.[Security Asset Name] IN ('Commodity Future Option') THEN 
					CASE WHEN C.[Underlying Symbol] = 'CL' THEN 'Crude oil derivatives'
					WHEN C.[Underlying Symbol] = 'LCO' THEN 'Crude oil derivatives'
					WHEN C.[Underlying Symbol] = 'GC' THEN 'Gold derivatives'
					WHEN C.[Underlying Symbol] = 'NG' THEN 'Natural Gas derivatives'
		--			WHEN C.[Underlying Symbol] = '???' THEN 'Power derivatives'
					WHEN C.[Underlying Symbol] IN ('C','HG','HO','SB','SI','W','XB','2RB','3C') THEN 'Other commodities derivatives'
					ELSE NULL
					END
				ELSE NULL
				END
			 WHEN C.[Security Asset Class] IN ('Swap') THEN
				CASE WHEN C.[Strategy] = 'STUBS' THEN 'Derivatives of unlisted equity'
				WHEN C.[Security Asset Name] IN ('Equity Swap', 'Index Swap') THEN 'Derivatives of listed equity'
				WHEN C.[Security Asset Name] IN ('IR/Currency Swap') THEN 'Interest rate derivatives'
				ELSE NULL
				END
			 ELSE NULL
			 END AS [Instrument_Category]




			,CASE WHEN C.[Repo] IS NOT NULL THEN 'Repo' ELSE C.[Security Asset Name] END AS [Instrument_Type]
			
			,CASE WHEN C.[Security Asset Class] IN ('Option') THEN 'Yes' 
				ELSE 'No' 
				END AS [Option_Instrument]
			
			,CASE WHEN C.[Repo] IS NOT NULL AND C.[Issue CCY] IN ('USD') THEN C.[Issue Price]
			 ELSE C.[Base Price]
			 END AS [Price]
			
			,C.[Issue Price] AS [Local_Price]
			,C.[Quantity(End)] AS [Quantity]
			
			,CASE 
			 WHEN C.[Repo] IS NOT NULL THEN
				CASE WHEN C.[Base End Loan Amount] >= 0 THEN 'Long'
				WHEN C.[Base End Loan Amount] < 0 THEN 'Short'
				ELSE NULL
				END
			 WHEN C.[Option Put/Call Flag] IN ('P') THEN
				CASE WHEN C.[Quantity(End)] > 0 THEN 'Short'
				ELSE 'Long'
				END
			 WHEN C.[Long/Short] IS NULL THEN
				CASE WHEN C.[Base Market Value] >= 0 THEN 'Long'
				WHEN C.[Base Market Value] < 0 THEN 'Short'
				WHEN C.[Base Market Value] IS NULL THEN
					CASE WHEN C.[Quantity(End)] >= 0 THEN 'Long'
					WHEN C.[Quantity(End)] < 0 THEN 'Short'
					ELSE NULL
					END
				ELSE NULL
				END
			 ELSE C.[Long/Short] 
			 END AS [LongShort]

			,CASE
				WHEN C.[Repo] IS NOT NULL THEN C.[Base End Loan Amount]
				WHEN C.[Security Asset Name] IN
					(
					'IR Future Option', 
					'IR Future Option - Short Term'
					) AND C.[Exchange Symbol] IN ('LIF') THEN (C.[Base Market Value] - C.[Base Total Cost])
				WHEN C.[Security Asset Name] IN 
					(
					'Commodity Future',
					'Cross Rate',
					'Currency',
					'Currency Future',
					'Index Future',
					'IR Future',
					'IR Future - Short Term',
					'IR/Currency Swap',
					'Single Stock Future'
					) THEN (C.[Base Market Value] - C.[Base Total Cost])
				--WHEN C.[Security Asset Name] IN ('Equity Swap','Index Swap') THEN COALESCE(C.[Base NAV Contribution],C.[Base Market Value],0)  --(COALESCE(C.[Base Market Value],0) - COALESCE(C.[Base Total Cost],0) + COALESCE(C.[Base Commissions],0))
				WHEN C.[Security Asset Name] IN ('Equity Swap') THEN COALESCE(C.[Base NAV Contribution],0) -- 2/21/2014 ATC Split the two for 500001773
				WHEN C.[Security Asset Name] IN ('Index Swap') THEN COALESCE(C.[Base NAV Contribution],C.[Base Market Value],0)
				ELSE C.[Base Market Value]
				END AS [Market_Value]
			 
			,CASE 
				WHEN C.[Repo] IS NOT NULL THEN 'NULL'
				WHEN C.[Security Asset Name] IN ('Bank Debt','CMO') THEN NULL --NEED 10-YEAR EQUIVALENTS FOR THESE
				WHEN C.[Security Asset Name] IN 
					(
					'Equity Swap',
					'Cross Rate',
					'Currency'
					) THEN C.[Base Market Value]
				WHEN C.[Security Asset Name] IN 
					(
					'IR/Currency Swap',
					'Credit Default Swap',
					'OTC FX Option',
					'Commodity Swaption',
					'OTC Bond Option'
					) THEN (C.[Quantity(End)] * (CASE WHEN C.[Issue CCY] IN ('EUR','GBP','AUD','NZD') THEN C.[Spot FX Rate] ELSE 1/C.[Spot FX Rate] END))
				WHEN C.[Security Asset Name] IN 
					(
					'Equity Option',
					'Index Option',
					'Index Future Option',
					'Commodity Future Option',
					'Currency Future Option'
					) THEN (C.[Quantity(End)] * (CASE WHEN C.[Issue CCY] IN ('EUR','GBP','AUD','NZD') THEN C.[Spot FX Rate] ELSE 1/C.[Spot FX Rate] END) * C.[Underlying Price] * C.[Contract Size])--C.[Price Factor]) -- Should be [Contract Size] instead of [Price Factor].  Need to bring [Contract Size] in from the Citco file -ATC 4/28/14
				--WHEN C.[Security Asset Name] IN
				--	(
				--	'IR Future Option',
				--	'IR Future Option - Short Term'
				--	) THEN (C.[Quantity(End)] * C.[Underlying Price] / 100 * DUR.[Underlier Contract Value] * (CASE WHEN C.[Issue CCY] IN ('EUR','GBP','AUD','NZD') THEN C.[Spot FX Rate] ELSE 1/C.[Spot FX Rate] END) * DUR.[Duration] / DUR.[10 Year Duration])
				--WHEN C.[Security Asset Name] IN ('IR Future') THEN (C.[Base Market Value] * DUR.[Duration] / DUR.[10 Year Duration])
				WHEN C.[Security Asset Name] IN ('Index Swap') THEN C.[Base Market Value]
				WHEN C.[Security Asset Class] = 'Future' THEN (C.[Base Market Value])
				ELSE NULL
				END AS [Notional_Value]

			,C.[End Date Base Unrealized P&L] AS [Unrealized_GL]

			,CP.[Issuer_Counterparty] AS [Issuer_Counterparty]

			--,COALESCE(CASE WHEN C.[Repo] IS NOT NULL THEN NULL ELSE O.[Issuer_Counterparty_Type] END,
			,CASE WHEN C.[Repo] IS NOT NULL THEN 'NULL'
			 WHEN C.[Security Asset Class] IN ('Equity','Option','Swap','Future') THEN
				CASE WHEN COALESCE(C.[Industry],OU.[Industry]) IN ('Financials') THEN 'Financial Institutions'
				WHEN COALESCE(C.[Industry],OU.[Industry]) = '*None*' THEN NULL
				WHEN COALESCE(C.[Industry],OU.[Industry]) IS NOT NULL THEN 'Other'
				ELSE NULL
				END
			 WHEN C.[Security Asset Name] IN ('Corporate Bond','Convertible Bond') THEN
				CASE WHEN COALESCE(C.[Industry],BU.[Industry]) IN ('Financials') THEN 'Financial Institutions'
				WHEN COALESCE(C.[Industry],BU.[Industry]) = '*None*' THEN NULL
				WHEN COALESCE(C.[Industry],BU.[Industry]) IS NOT NULL THEN 'Other'
				ELSE NULL
				END
			 ELSE NULL
			 END AS [Issuer_Counterparty_Type]
			
			,CASE WHEN C.[Security Asset Name] = 'ACCOUNTING CASH' THEN CCY.[Issuer_Counterparty_Country]
				ELSE R.[ISO_3166-1_alpha-3] 
				END AS [Issuer_Counterparty_Country]
			
			,CP.[Issuer_Counterparty_LegalName] AS [Issuer_Counterparty_LegalName]

			,CP.[Issuer_CounterParty_Affiliate] AS [Issuer_CounterParty_Affiliate]
			
			,CP.[Issuer_Counterparty_Affiliate_Other] AS [Issuer_CounterParty_Affiliate_Other]

		----------  Investment Quality  ----------
			--,COALESCE(CASE WHEN C.[Repo] IS NOT NULL THEN NULL ELSE O.[Investment_Quality] END,
			,CASE WHEN C.[Repo] IS NOT NULL THEN 'NULL'
			 WHEN C.[Security Asset Name] IN ('Corporate Bond','Convertible Bond') THEN 
				CASE WHEN SP.[Quality] = 'Non-IG' AND M.[Quality] = 'IG' THEN 'Non-IG'
				WHEN M.[Quality] = 'Non-IG' AND SP.[Quality] = 'IG' THEN 'Non-IG'
				ELSE COALESCE(SP.[Quality],M.[Quality],'Non-IG')
				END
			 ELSE 'NULL'
			 END AS [Investment_Quality]

			,C.[Option Delta] AS [Delta]
				-- CASE WHEN C.[Option Put/Call Flag] = 'C' THEN 0.5
				--	WHEN C.[Option Put/Call Flag] = 'P' THEN -0.5
				--	ELSE 1
				--	END) 

			,NULL AS [Vega]
			,NULL AS [DV01]

			,CASE WHEN C.[Repo] IS NOT NULL THEN 'Duration'
				WHEN C.[Security Asset Class] IN ('Bond') AND C.[Security Asset Name] NOT IN ('Credit Default Swap')THEN 'Duration'
	 			ELSE NULL
				END AS [IR_Sensitivity_Measure]
			 
			,NULL AS [IR_Sensitivity_Measure_Value]
			
			,CASE WHEN C.[Repo] IS NOT NULL THEN NULL
				 ELSE CASE
					WHEN C.[Standard Strategy] = 'D' THEN 'Event Driven, Distressed/Restructuring'
					WHEN C.[Standard Strategy] = 'M' THEN 'Event Driven, Risk Arbitrage/Merger Arbitrage'
					WHEN C.[Standard Strategy] = 'S' THEN 'Event Driven, Equity Special Situations'
					WHEN C.[Standard Strategy] = 'H' THEN 'Other'
					WHEN C.[Standard Strategy] = 'C' THEN 'Other'
					WHEN C.[Security Asset Name] = 'ACCOUNTING CASH' THEN 'Other'
					ELSE NULL
					END
				 END AS [Strategy] 
			 
			 ,CASE 
				WHEN C.[Standard Strategy] = 'H' THEN 'Portfolio Hedges'
				WHEN C.[Standard Strategy] = 'C' THEN 'Currency'
				WHEN C.[Security Asset Name] = 'ACCOUNTING CASH' THEN 'Currency'
				ELSE NULL
				END AS [Strategy_Name_Other]
			
			,'No' AS [High_Freq_Trading_Indicator]
			,L.[Liquidation_Horizon_Days]
			
		
		
		
			,CASE 
				WHEN C.[Prime Broker/Clearing Broker] IN
							('JPM-ICE') THEN 'Yes' -- Added 2/18/2014 by ATC to capture JPM-ICE CDS

				WHEN C.[Security Asset Name] IN 
					(
					'Credit Default Swap',
					'Currency',
					'Equity Swap',
					'Index Swap'
					) THEN 'No'

-- Changed 3/19/2013 by ATC for GNC 2013-03-27 12.100 CALL  (OTC)
				--WHEN C.[Security Asset Name] IN
				--	(
				--	'Equity Option',
				--	'Index Option',
				--	'IR Future Option - Short Term'
				--	) THEN CASE
				WHEN C.[Security Asset Class] IN
					(
					'Option'
					) THEN CASE
						WHEN C.[Exchange Symbol] IN 
							(
							'OTC',
							'PNK',
							'OBB'
							) 
-- Added 3/19/2013 by ATC for GNC 2013-03-27 12.100 CALL  (OTC)
							OR 
							C.[Security Description] LIKE '%OTC%' THEN 'No'
						WHEN C.[Exchange Symbol] = 'LIF' THEN CASE
								WHEN C.[Security Asset Name] = 'IR Future Option - Short Term' THEN 'Yes'
								ELSE 'No'
								END
						ELSE 'Yes'
						END
				ELSE 'Yes'
				END AS [Cleared_by_a_CCP]

	
	
	
	
	
	
			,CASE WHEN CP.[Issuer_Counterparty] = 'Lehman Claim' THEN 'Misc' ELSE 'NULL' END AS [AssetClass_Other]

			,NULL AS [Value]

			,NULL AS [Exclude_Issuer_Counterparty]
			,NULL AS [Exclude_Strategy]
			,NULL AS [Exclude_Issuer_Counterparty_Country]


			,C.[Citco Security ID] AS [Symbol]
			,C.[Security Description] AS [Instrument_Description]
			,'NULL' AS [Region]
			
--********* Additional Duration Fields *********--

			,C.[Settle Days]
			,CONVERT(DateTime,C.[Period End Date],101) + C.[Settle Days] AS [Settlement]
			,CONVERT(DateTime,C.[Maturity Date],101) AS [Maturity]
			,C.[Coupon Rate]/100 AS [Coupon/Rate]
			,C.[Redeem Price]
			,ROUND(365/C.[Daycount],0) AS [Frequency]
			,C.[Day Count Fraction Repo]
			,DC.[Basis]




		FROM [FormPF_Input].[dbo].[Gruss_StdPos_ME] C
		--RRPRDSQL01.ClientStage.dbo.Citco_SPOS C

			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_StrtTyp] S
				ON C.[Trader]= S.[Trader]
			
			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_SP_Rtg] SP
				ON C.[S&P Bond Rating] = SP.[SP_Rtg]
			
			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_Moody_Rtg] M
				ON C.[Moody Bond Rating] = M.[Moody_Rtg]
			
			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_CntryRgn] R
				ON C.[Country Code] = R.[ISO_3166-1_alpha-2]

			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_IssueCcy] CCY
				ON C.[Security Asset Name] = 'ACCOUNTING CASH'
				AND C.[Issue Ccy] = CCY.[Issue Ccy]
			
			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_CntrPrty] CP
				ON C.[Prime Broker/Clearing Broker] = CP.[Prime Broker/Clearing Broker]

			LEFT JOIN [FormPF_Mapping].[dbo].[Gruss_Map_DayCount] DC
				ON C.[Day Count Fraction Repo] = DC.[Day Count Fraction Repo]
			
			LEFT JOIN [FormPF_Input].[dbo].[Gruss_Liquidation_Horizon] L
				ON L.[Period End Date] = C.[Period End Date]
				AND L.[SubFund] = C.[SubFund]
				AND L.[Trader] = C.[Trader]
				AND L.[Prime Broker/Clearing Broker] = C.[Prime Broker/Clearing Broker]
				AND L.[Citco Security ID] = C.[Citco Security ID]
				AND L.[Security Asset Class] = C.[Security Asset Class]
				AND L.[Security Asset Name] = C.[Security Asset Name]
				AND L.[Quantity(End)] = C.[Quantity(End)]


			LEFT JOIN Underlying_Industry_CTE OU
				ON CAST(C.[Period End Date] AS Date) = OU.[Period End Date]
				AND C.[Underlying Symbol] = OU.[Symbol]
				AND C.[Underlying Citco Security ID] = CAST(OU.[Citco Security ID] AS nvarchar)
				AND C.[Security Asset Name] IN ('Equity','Private Placement','Single Stock Future','Index Future','Equity Option','Index Option','Index Future Option','Equity Swap')
				AND C.[Security Asset Class] IN ('Equity','Option','Swap','Future')
			
			LEFT JOIN Underlying_Industry_CTE BU
				ON C.[Period End Date] = BU.[Period End Date]
				AND C.[Bond Abbrev] = BU.[Symbol]
				AND C.[Security Asset Name] IN ('Corporate Bond','Convertible Bond')
			
			LEFT JOIN [FormPF_Input].[dbo].[Gruss_Deltas] D
				ON CAST(C.[Period End Date] AS Date) = D.[Trade Date (P/L)]
				AND C.[Citco Security ID] = D.[Id]
				AND C.[Trader] = D.[Trader]
					
		WHERE 
			(C.[Quantity(End)] <> '0' OR C.[Repo] IS NOT NULL)
			AND CONVERT(DateTime,C.[Period End Date],101) BETWEEN @sDate AND @eDate
			AND C.[Security Asset Name] NOT IN ('Internal Fund')
			--AND C.[Security Asset Name] NOT IN ('ACCOUNTING CASH')
		)



------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

SELECT 
	'NULL' AS [ID],
	P.[PFA_Unique_Identifier],
	P.[Fund_Unique_Identifier],
	P.[Business_Date],
	P.[Information_Date],
	P.[Position_Unique_Identifier],
	COALESCE(O.[Instrument_Group],P.[Instrument_Group]) AS [Instrument_Group],
	COALESCE(O.[Instrument_Category],P.[Instrument_Category]) AS [Instrument_Category],
	COALESCE(O.[Instrument_Type],P.[Instrument_Type]) AS [Instrument_Type],
	COALESCE(O.[Option_Instrument],P.[Option_Instrument]) AS [Option_Instrument],
	COALESCE(O.[Price],P.[Price]) AS [Price],
	COALESCE(O.[Local_Price],P.[Local_Price]) AS [Local_Price],
	COALESCE(O.[Quantity],P.[Quantity]) AS [Quantity],
	COALESCE(O.[LongShort],P.[LongShort]) AS [LongShort],
	COALESCE(O.[Market_Value],P.[Market_Value]) AS [Market_Value],
	COALESCE(O.[Notional_Value],P.[Notional_Value]) AS [Notional_Value],
	--P.[Unrealized_GL],
	COALESCE(O.[Issuer_Counterparty],P.[Issuer_Counterparty]) AS [Issuer_Counterparty],
	
	CASE WHEN COALESCE(O.[Instrument_Category],P.[Instrument_Category]) NOT IN (
		'Convertible bonds',
		'Corporate non-convertible bonds',
		'Derivatives of listed equity',
		'Derivatives of unlisted equity',
		'Listed Equity',
		'Unlisted Equity') THEN 'NULL'
		ELSE COALESCE(O.[Issuer_Counterparty_Type],P.[Issuer_Counterparty_Type])
		END AS [Issuer_Counterparty_Type],
	
	COALESCE(O.[Issuer_Counterparty_Country],P.[Issuer_Counterparty_Country]) AS [Issuer_Counterparty_Country],
	
	CASE WHEN COALESCE(O.[Issuer_Counterparty],P.[Issuer_Counterparty]) IS NULL THEN 'NA' ELSE COALESCE(O.[Issuer_Counterparty_LegalName],P.[Issuer_Counterparty_LegalName]) END AS [Issuer_Counterparty_LegalName],
	
	CASE WHEN COALESCE(O.[Issuer_Counterparty],P.[Issuer_Counterparty]) IS NULL THEN 'NA' ELSE COALESCE(O.[Issuer_CounterParty_Affiliate],P.[Issuer_CounterParty_Affiliate]) END AS [Issuer_CounterParty_Affiliate],
	
	COALESCE(O.[Issuer_Counterparty_Affiliate_Other],P.[Issuer_Counterparty_Affiliate_Other]) AS [Issuer_Counterparty_Affiliate_Other],
	
	CASE WHEN COALESCE(O.[Instrument_Category],P.[Instrument_Category]) NOT IN (
		'Convertible bonds',
		'Corporate non-convertible bonds') THEN 'NULL'
		ELSE COALESCE(O.[Investment_Quality],P.[Investment_Quality])
		END AS [Investment_Quality],
	COALESCE(O.[Delta],P.[Delta],0) AS [Delta],
	0 AS [Vega],
	0 AS [DV01],
	CASE WHEN COALESCE(O.[Instrument_Category],P.[Instrument_Category]) NOT IN (
		'ABCP',
		'CDO/CLO',
		'MBS',
		'Other ABS',
		'Agency securities',
		'Convertible bonds',
		'Corporate non-convertible bonds',
		'G10 non-US sovereign bonds',
		'GSE bonds',
		'Leveraged loans',
		'Other loans',
		'Other sovereign and supranational bonds',
		'U.S. state and local bonds',
		'U.S. treasury securities',
		'Repos') THEN 'NULL' ELSE COALESCE(O.[IR_Sensitivity_Measure],P.[IR_Sensitivity_Measure]) END AS [IR_Sensitivity_Measure],

	CASE WHEN COALESCE(O.[Instrument_Category],P.[Instrument_Category]) NOT IN (
		'ABCP',
		'CDO/CLO',
		'MBS',
		'Other ABS',
		'Agency securities',
		'Convertible bonds',
		'Corporate non-convertible bonds',
		'G10 non-US sovereign bonds',
		'GSE bonds',
		'Leveraged loans',
		'Other loans',
		'Other sovereign and supranational bonds',
		'U.S. state and local bonds',
		'U.S. treasury securities',
		'Repos') THEN NULL ELSE COALESCE(O.[IR_Sensitivity_Measure_Value],P.[IR_Sensitivity_Measure_Value],0) END AS [IR_Sensitivity_Measure_Value],
	
	COALESCE(O.[Strategy],P.[Strategy]) AS [Strategy],
	COALESCE(O.[Strategy_Name_Other],P.[Strategy_Name_Other],'NULL') AS [Strategy_Name_Other],
	COALESCE(O.[High_Freq_Trading_Indicator],P.[High_Freq_Trading_Indicator]) AS [High_Freq_Trading_Indicator],
	COALESCE(O.[Liquidation_Horizon_Days],P.[Liquidation_Horizon_Days],0) AS [Liquidation_Horizon_Days],
	COALESCE(O.[Cleared_by_a_CCP],P.[Cleared_by_a_CCP]) AS [Cleared_by_a_CCP],
	COALESCE(O.[AssetClass_Other],P.[AssetClass_Other],'NULL') AS [AssetClass_Other],
	CASE
		WHEN COALESCE(O.[Option_Instrument], P.[Option_Instrument]) IN ('Yes','Y') THEN ABS(COALESCE(O.[Notional_Value],P.[Notional_Value]) * COALESCE(O.[Delta],P.[Delta],0))
		WHEN COALESCE(O.[Instrument_Group],P.[Instrument_Group]) LIKE '%derivative%' THEN ABS(COALESCE(O.[Notional_Value],P.[Notional_Value]))
		ELSE ABS(COALESCE(O.[Market_Value],P.[Market_Value]))
		END AS [Value],

	CASE WHEN COALESCE(O.[Issuer_Counterparty],P.[Issuer_Counterparty]) IS NULL THEN 'Yes' ELSE 'No' END AS [Exclude_Issuer_Counterparty],
	CASE WHEN COALESCE(O.[Strategy],P.[Strategy]) IS NULL THEN 'Yes' ELSE 'No' END AS [Exclude_Strategy],
	CASE WHEN COALESCE(O.[Issuer_Counterparty_Country],P.[Issuer_Counterparty_Country]) IS NULL THEN 'Yes' ELSE 'No' END AS [Exclude_Issuer_Counterparty_Country],
	COALESCE(O.[Symbol],P.[Symbol]) AS [Symbol],
	COALESCE(O.[Instrument_Description],P.[Instrument_Description]) AS [Instrument_Description],
	COALESCE(O.[Region],P.[Region]) AS [Region],
	'NULL' AS [Any_Securities_Borrowing_Lending]

--********* Additional Duration Fields *********--

	,P.[Settle Days]
	,CAST(P.[Settlement] AS date) AS [Settlement]
	,CAST(P.[Maturity] AS date) AS [Maturity]
	,P.[Coupon/Rate]
	,P.[Redeem Price]
	,P.[Frequency]
	,P.[Day Count Fraction Repo]
	,P.[Basis]

FROM 
	Position_Level_CTE P

	LEFT JOIN [FormPF_Input].[dbo].[Gruss_Overrides] O
		ON O.[Business_Date] = P.[Business_Date]
		AND O.[Fund_Unique_Identifier] = P.[Fund_Unique_Identifier]
		AND O.[Symbol] = P.[Symbol]

	--WHERE
	--	P.[Issuer_Counterparty_Country] IS NULL