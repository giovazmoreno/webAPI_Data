USE [Promarket_POSDATA]
GO
/****** Object:  StoredProcedure [dbo].[spWS_PosData_PMKTData]    Script Date: 28/01/2019 15:25:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[spWS_PosData_PMKTData]
-- Add the parameters for the stored procedure here
	@Action VARCHAR(50),
	@nPER_clave BIGINT = NULL ,
	@cPER_clave VARCHAR(10) = NULL,
	@cPER_nombres VARCHAR(35) = NULL,
	@cPER_apellidopaterno VARCHAR(35) = NULL,
	@cPER_apellidomaterno VARCHAR(35) = NULL,
	@nPER_estatus VARCHAR(20) = NULL ,
	@cPRO_clave VARCHAR(10) = NULL,
	@dPER_fechaactualizacion DATETIME = NULL,
	@cPER_actualizadopor VARCHAR(100) = NULL,
	@nTIP_clave VARCHAR(5) = NULL,
	@nPER_supervisor BIGINT = NULL,
	@cPER_usuariored VARCHAR(20) = NULL,
	@cPER_cuentacorreo VARCHAR(100) = NULL,
	@cPER_contrasena VARCHAR(10) = NULL,
	@nEPS_claveempresa INT = NULL,
	@cCLI_clave VARCHAR(10) = NULL,
	@cTIP_nombre VARCHAR(20) = NULL,
	@nTIM_clave VARCHAR(5) = NULL ,
	@cCAP_clavecategoria VARCHAR(3) = NULL ,
	@cRUT_clave VARCHAR(10) = NULL,
	@nDIP_clave INT = NULL,
	@cMNV_clave VARCHAR(5) = NULL,
	@nSIN_clave BIGINT = NULL,
	@cREG_clave VARCHAR(5) = NULL,
	@cREG_nombre VARCHAR(50) = NULL,
	@cCIU_clave VARCHAR(5) = NULL,
	@cCIU_nombre VARCHAR(50) = NULL,
	@cTIE_clave VARCHAR(10) = NULL,
	@bSIN_checkin BIT = NULL,
	@cSIN_checkinlatitud VARCHAR(20) = NULL,
	@cSIN_checkinlongitud VARCHAR(20) = NULL,
	@dSIN_checkinfecha VARCHAR(30) = NULL,
	@dSIN_checkinhora VARCHAR(30) = NULL,
	@bSIN_checkout BIT = NULL,
	@cSIN_checkoutlatitud VARCHAR(20) = NULL,
	@cSIN_checkoutlongitud VARCHAR(20) = NULL,
	@dSIN_checkoutfecha VARCHAR(30) = NULL,
	@dSIN_checkouthora VARCHAR(30) = NULL,
	@cSIN_checkoutobservaciones VARCHAR(300) = NULL,
	@cENC_clave VARCHAR(5) = NULL,
	@bENC_respuesta1 BIT = NULL,
	@bENC_respuesta2 BIT = NULL,
	@bENC_respuesta3 BIT = NULL,
	@bENC_respuesta4 BIT = NULL,
	@bENC_respuesta5 BIT = NULL,
	@bENC_respuesta6 BIT = NULL,
	@bENC_respuesta7 BIT = NULL,
	@bENC_respuesta8 BIT = NULL,
	@bENC_respuesta9 BIT = NULL,
	@bENC_respuesta10 BIT = NULL,
	@bENC_respuesta11 BIT = NULL,
	@bENC_respuesta12 BIT = NULL,
	@bENC_respuesta13 BIT = NULL,
	@bENC_respuesta14 BIT = NULL,
	@bENC_respuesta15 BIT = NULL,
	@cENC_comentarios VARCHAR(300) = NULL,
	@dENC_horainicio VARCHAR(30) = NULL,
	@dENC_horatermino VARCHAR(30) = NULL,
	@NombreRuta VARCHAR(30) = NULL,
	@Supervisor VARCHAR(20) = NULL,
	@ProfileName VARCHAR(30) = NULL,
	@BodyEmailReset VARCHAR(MAX) = NULL,
	@TipoFueraRuta INT = NULL
AS
BEGIN
	DECLARE @LocalnPER_clave BIGINT = NULL;
	SET @LocalnPER_clave = @nPER_clave
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Insert statements for procedure here
	IF (@Action = 'ValidateUser')
	BEGIN
	    OPEN SYMMETRIC KEY SymKey_PWD
	    DECRYPTION BY CERTIFICATE PWD_Certificate;
	    
	    SELECT p.nPER_clave,
	           p.nPER_estatus,
	           p.nTIP_clave,
	           (
	               SELECT '1'
	               FROM   [dbo].[PMC_Personal_LIC_Licenciamiento] AS pl
	               WHERE  [bETL_estatus] = 1
	                      AND p.nPER_clave = pl.nPER_clave
	           )                     AS Licencia
	    FROM   [dbo].[PMC_Personal]  AS p
	           INNER JOIN [dbo].[BO_TipoPersonal] AS tp
	                ON  p.nTIP_clave = tp.nTIP_clave
	    WHERE  cPER_cuentacorreo = @cPER_cuentacorreo
	           AND CONVERT(VARCHAR(10), DECRYPTBYKEY([cPER_contrasena])) = @cPER_contrasena
	    
	    CLOSE SYMMETRIC KEY SymKey_PWD;
	END
	ELSE 
	IF (@Action = 'GetGeneralDataUser')
	BEGIN
	    SELECT nPER_clave,
	           cPER_clave,
	           cPER_nombres,
	           cPER_apellidopaterno,
	           cPER_apellidomaterno,
	           nPER_estatus,
	           p.nTIP_clave,
	           tp.cTIP_nombre,
	           nPER_supervisor,
	           cPER_usuariored,
	           cPER_cuentacorreo,
	           cPER_contrasena
	    FROM   [dbo].[PMC_Personal] AS p
	           INNER JOIN [dbo].[BO_TipoPersonal] AS tp
	                ON  p.nTIP_clave = tp.nTIP_clave
	    WHERE  nPER_clave = @nPER_clave
	END
	ELSE 
	IF (@Action = 'GetListPromosUser')
	BEGIN
	    SELECT pp.cPRO_clave,
	           pp.cCLI_clave,
	           pp.nEPS_claveempresa,
	           pp.nPER_clave,
	           c.[cCLI_nombre],
	           promo.[cPRO_nombre],
	           ISNULL(promo.[bPRO_validarcheckin], '0') AS bPRO_validarcheckin,
			   promo.cPRO_evidenciafotografica
	    FROM   [dbo].[PMC_Promocion_PMC_Personal] AS pp
	           INNER JOIN [dbo].[PMC_Promocion] AS promo
	                ON  pp.cPRO_clave = promo.cPRO_clave
	                AND pp.nEPS_claveempresa = promo.nEPS_claveempresa
	           INNER JOIN [dbo].[PMC_Cliente] AS c
	                ON  pp.cCLI_clave = c.cCLI_clave
	                AND pp.nEPS_claveempresa = c.nEPS_claveempresa
	    WHERE  pp.nPER_clave = @nPER_clave
	END
	ELSE 
	IF (@Action = 'GetEventsForPromo')
	BEGIN
	    SELECT pe.cPRO_clave,
	           pe.nTIM_clave,
	           e.[cTIM_nombre]
	    FROM   [dbo].[PMC_Promocion_PMC_TipoEvento] AS pe
	           INNER JOIN [dbo].[PMC_TipoEvento] AS e
	                ON  e.[nTIM_clave] = pe.[nTIM_clave]
	    WHERE  e.[nTIM_estatus] = 1
	           AND pe.cPRO_clave = @cPRO_clave
	END
	ELSE 
	IF (@Action = 'ActiveModulesPromo')
	BEGIN
	    SELECT pppte.cPRO_clave,
	           pppte.nTIM_clave,
	           pte.cTIM_nombre
	    FROM   PMC_Promocion_PMC_TipoEvento AS pppte
	           INNER JOIN PMC_TipoEvento AS pte
	                ON  pte.nTIM_clave = pppte.nTIM_clave
	                AND pte.nTIM_estatus = 1
	    WHERE  pppte.cPRO_clave = @cPRO_clave
	    ORDER BY
	           pte.nTIM_clave
	END
	ELSE 
	IF (@Action = 'DetailEvents')
	BEGIN
	    --MotivosNoVisita
	    --SELECT pmnv.cMNV_clave,
	    --       pmnv.cMNV_nombre
	    --FROM   PMC_MotivosNoVisita AS pmnv
	    --WHERE  pmnv.nMNV_estatus = 1
	    SELECT PMC_MotivosNoVisita.cMNV_clave,
	           PMC_MotivosNoVisita.cMNV_nombre
	    FROM   PMC_Promocion_PMC_MotivosNoVisita
	           INNER JOIN PMC_MotivosNoVisita
	                ON  PMC_Promocion_PMC_MotivosNoVisita.cMNV_clave = 
	                    PMC_MotivosNoVisita.cMNV_clave
	    WHERE  (PMC_Promocion_PMC_MotivosNoVisita.cPRO_clave = @cPRO_clave)
	           AND (PMC_MotivosNoVisita.nMNV_estatus = 1)
	    ORDER BY
	           PMC_MotivosNoVisita.cMNV_nombre
	    
	    --Encuestas
	    SELECT pe.cENC_clave,
	           pe.cENC_nombre,
	           pe.cENC_pregunta1,
	           pe.cENC_pregunta2,
	           pe.cENC_pregunta3,
	           pe.cENC_pregunta4,
	           pe.cENC_pregunta5,
	           pe.cENC_pregunta6,
	           pe.cENC_pregunta7,
	           pe.cENC_pregunta8,
	           pe.cENC_pregunta9,
	           pe.cENC_pregunta10,
	           pe.cENC_pregunta11,
	           pe.cENC_pregunta12,
	           pe.cENC_pregunta13,
	           pe.cENC_pregunta14,
	           pe.cENC_pregunta15
	    FROM   PMC_Encuestas AS pe
	    WHERE  pe.bENC_estatus = 1
	           AND pe.cPRO_clave = @cPRO_clave
	    
	    --EvidenciaFotografica
	    SELECT ppptef.nTEF_clave,
	           ptef.cTEF_nombre
	    FROM   PMC_Promocion_PMC_TipoEvidenciaFotografica AS ppptef
	           INNER JOIN PMC_TipoEvidenciaFotografica AS ptef
	                ON  ptef.nTEF_clave = ppptef.nTEF_clave
	                AND ptef.nTEF_estatus = 1
	    WHERE  ppptef.cPRO_clave = @cPRO_clave
	    
	    --Productos
	    SELECT pp.cPRD_claveproducto,
	           pp.cPRD_nombre,
	           pp.cCAP_clavecategoria,
	           pcp.cCAP_nombre
	    FROM   PMC_Productos AS pp
	           INNER JOIN PMC_CategoriaProductos AS pcp
	                ON  pcp.cCAP_clavecategoria = pp.cCAP_clavecategoria
	                AND pcp.cPRO_clave = pp.cPRO_clave
	                AND pcp.nCAP_estatus = 1
	    WHERE  pp.nPRD_estatus = 1
	           AND pp.cPRO_clave = @cPRO_clave
	    
	    --TipoMaterial
	    SELECT ppptm.cTIM_clave,
	           ptm.cTIM_nombre
	    FROM   PMC_Promocion_PMC_TipoMaterial AS ppptm
	           INNER JOIN PMC_TipoMaterial AS ptm
	                ON  ptm.cTIM_clave = ppptm.cTIM_clave
	                AND ptm.nTIM_estatus = 1
	    WHERE  ppptm.cPRO_clave = @cPRO_clave
	    
	    --UbicacionProducto
	    SELECT pppup.cUPR_clave,
	           pup.cUPR_nombre
	    FROM   PMC_Promocion_PMC_UbicacionProducto AS pppup
	           INNER JOIN PMC_UbicacionProducto AS pup
	                ON  pup.cUPR_clave = pppup.cUPR_clave
	                AND pup.nUPR_estatus = 1
	    WHERE  pppup.cPRO_clave = @cPRO_clave
	    
	    --KPI
	    SELECT pppk.cKPI_clave,
	           pk.cKPI_nombre
	    FROM   PMC_Promocion_PMC_KPI  AS pppk
	           INNER JOIN PMC_KPI     AS pk
	                ON  pk.cKPI_clave = pppk.cKPI_clave
	                AND pk.cKPI_estatus = 1
	    WHERE  pppk.cPRO_clave = @cPRO_clave
	END
	ELSE 
	IF (@Action = 'DataRoute')
	BEGIN
	    SET @nPER_clave = (
	            SELECT TOP 1 [nPER_clave]
	            FROM   [dbo].[PMC_Personal]
	            WHERE  LTRIM(RTRIM([cPER_cuentacorreo])) = @cPER_cuentacorreo
	        );
	    
	    SET @cRUT_clave = (
	            SELECT TOP 1
	                   rp.cRUT_clave
	            FROM   [dbo].[PMC_Ruta_PMC_Personal] AS rp
	                   INNER JOIN [dbo].[PMC_Ruta] AS r
	                        ON  r.[cRUT_clave] = rp.[cRUT_clave]
	                        AND r.[cPRO_clave] = rp.[cPRO_clave]
	                   INNER JOIN [dbo].[PMC_Promocion] AS p
	                        ON  p.[cPRO_clave] = rp.[cPRO_clave]
	                        AND p.[nEPS_claveempresa] = rp.[nEPS_claveempresa]
	                        AND p.[cCLI_clave] = rp.[cCLI_clave]
	            WHERE  rp.[cPRO_clave] = @cPRO_clave
	                   AND rp.nPER_clave = @nPER_clave
	                   AND r.[nRUT_estatus] = 1
	                   AND (rp.dRUT_fechatermino = '' OR rp.dRUT_fechatermino IS NULL)
	            ORDER BY
	                   r.[cRUT_nombre]
	        );
	    
	    SELECT @TipoFueraRuta = pp.nTFR_clave,
	           @cCLI_clave = pp.cCLI_clave
	    FROM   PMC_Promocion AS pp
	    WHERE  pp.cPRO_clave = @cPRO_clave
	    
	    
	    --General Data
	    SELECT rp.cRUT_clave,
	           r.[cRUT_nombre],
	           r.cREG_clave    AS cREG_clave,
	           rg.cREG_nombre  AS cREG_nombre,
	           r.cCIU_clave    AS cCIU_clave,
	           c.cCIU_nombre   AS cCIU_nombre,
	           (
	               CASE 
	                    WHEN p.[bPRO_supervision] = 1 THEN (
	                             SELECT per.[cPER_nombres] + ' ' + per.[cPER_apellidopaterno] 
	                                    + ' ' + ISNULL(per.[cPER_apellidomaterno], '')
	                             FROM   [dbo].[PMC_Personal] AS per
	                             WHERE  per.[nPER_clave] = r.[nPER_supervisor]
	                         )
	                    ELSE '-1'
	               END
	           )               AS Supervisor
	    FROM   [dbo].[PMC_Ruta_PMC_Personal] AS rp
	           INNER JOIN [dbo].[PMC_Ruta] AS r
	                ON  r.[cRUT_clave] = rp.[cRUT_clave]
	                AND r.[cPRO_clave] = rp.[cPRO_clave]
	           INNER JOIN [dbo].[PMC_Promocion] AS p
	                ON  p.[cPRO_clave] = rp.[cPRO_clave]
	                AND p.[nEPS_claveempresa] = rp.[nEPS_claveempresa]
	                AND p.[cCLI_clave] = rp.[cCLI_clave]
	           INNER JOIN [dbo].[PMC_Region] AS rg
	                ON  rg.[cREG_clave] = r.[cREG_clave]
	                AND rg.[cPRO_clave] = r.[cPRO_clave]
	           INNER JOIN [dbo].[BO_Ciudad] AS c
	                ON  c.[cCIU_clave] = r.[cCIU_clave]
	    WHERE  rp.[cPRO_clave] = @cPRO_clave
	           AND rp.nPER_clave = @nPER_clave
	           AND r.[nRUT_estatus] = 1
	           AND (rp.dRUT_fechatermino = '' OR rp.dRUT_fechatermino IS NULL)
	    ORDER BY
	           r.[cRUT_nombre];
	    
	    --Route_Day
	    SELECT t.[cTIE_clave],
	           t.[cTIE_nombre],
	           r.[cREG_clave],
	           r.[cREG_nombre],
	           c.[cCIU_clave],
	           c.[cCIU_nombre],
	           t.[cTIE_latitud],
	           t.[cTIE_longitud],
	           t.[nTIE_rangocheckin]
	    FROM   [dbo].[PMC_Ruta_Programacion] AS rp
	           INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS rpt
	                ON  rp.[cRUT_clave] = rpt.[cRUT_clave]
	                AND rp.[cPRO_clave] = rpt.[cPRO_clave]
	                AND rp.[nDIP_clave] = rpt.[nDIP_clave]
	           INNER JOIN [dbo].[PMC_Tienda] AS t
	                ON  t.[cPRO_clave] = rp.[cPRO_clave]
	                AND t.[cTIE_clave] = rpt.[cTIE_clave]
	                AND t.[cCIU_clave] = rpt.[cCIU_clave]
	                AND t.[cREG_clave] = rpt.[cREG_clave]
	                AND t.[nTIE_estatus] = 1
	           LEFT OUTER JOIN [dbo].[PMC_Region] AS r
	                ON  r.[cREG_clave] = t.[cREG_clave]
	                AND r.[cPRO_clave] = t.[cPRO_clave]
	           LEFT OUTER JOIN [dbo].[BO_Ciudad] AS c
	                ON  c.[cCIU_clave] = t.[cCIU_clave]
	    WHERE  rp.cRUT_clave = @cRUT_clave
	           AND rp.cPRO_clave = @cPRO_clave
	           AND rp.nDIP_clave = @nDIP_clave
	           AND t.[cTIE_clave] NOT IN (SELECT [cTIE_clave]
	                                      FROM   [dbo].[POD_Sincronizacion]
	                                      WHERE  cPRO_clave = @cPRO_clave
	                                             AND cRUT_clave = @cRUT_clave
	                                             AND rp.nDIP_clave = @nDIP_clave
	                                             AND CONVERT(DATE, [dSIN_fechainicio]) = 
	                                                 CONVERT(DATE, GETDATE()))
	    ORDER BY
	           rpt.[nRUT_ordenvisita]
	    
	    
	    --OutPointsRoute_Day
	    IF (@TipoFueraRuta = 1) --Region-Ciudad
	    BEGIN
	        --TODO: GET ALL THE ROUTE'S STOTES OF THE PROMO,  EXCEPT THE STORES THAT BELONG TO THE SCHEDULED ROUTE FOR THE CURRENT DAY
	        SELECT t.[cTIE_clave],
	               t.[cTIE_nombre],
	               r.[cREG_clave],
	               r.[cREG_nombre],
	               c.[cCIU_clave],
	               c.[cCIU_nombre],
	               t.[cTIE_latitud],
	               t.[cTIE_longitud],
	               t.[nTIE_rangocheckin]
	        FROM   [dbo].[PMC_Ruta_BO_Ciudad_PMC_Region] AS rcr
	               INNER JOIN [dbo].[PMC_Tienda] AS t
	                    ON  t.[cPRO_clave] = rcr.[cPRO_clave]
	                    AND t.[cREG_clave] = rcr.[cREG_clave]
	                    AND t.[cCIU_clave] = rcr.[cCIU_clave]
	               INNER JOIN [dbo].[PMC_Region] AS r
	                    ON  r.[cREG_clave] = rcr.[cREG_clave]
	                    AND r.[cPRO_clave] = rcr.[cPRO_clave]
	               INNER JOIN [dbo].[BO_Ciudad] AS c
	                    ON  c.[cCIU_clave] = rcr.[cCIU_clave]
	        WHERE  rcr.cRUT_clave = @cRUT_clave
	               AND rcr.cPRO_clave = @cPRO_clave
	               AND t.cTIE_clave NOT IN (SELECT t.[cTIE_clave]
	                                        FROM   [dbo].[PMC_Ruta_Programacion] AS 
	                                               rp
	                                               INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS 
	                                                    rpt
	                                                    ON  rp.[cRUT_clave] = 
	                                                        rpt.[cRUT_clave]
	                                                    AND rp.[cPRO_clave] = 
	                                                        rpt.[cPRO_clave]
	                                                    AND rp.[nDIP_clave] = 
	                                                        rpt.[nDIP_clave]
	                                               INNER JOIN [dbo].[PMC_Tienda] AS 
	                                                    t
	                                                    ON  t.[cPRO_clave] = rp.[cPRO_clave]
	                                                    AND t.[cTIE_clave] = rpt.[cTIE_clave]
	                                                    AND t.[cCIU_clave] = rpt.[cCIU_clave]
	                                                    AND t.[cREG_clave] = rpt.[cREG_clave]
	                                                    AND t.[nTIE_estatus] = 1
	                                        WHERE  rp.cRUT_clave = @cRUT_clave
	                                               AND rp.cPRO_clave = @cPRO_clave
	                                               AND rp.nDIP_clave = @nDIP_clave)
	        ORDER BY
	               t.[cTIE_nombre]
	    END
	    ELSE
	        --Programacion
	    BEGIN
	        SELECT cCLI_clave,
	               cCLI_nombre,
	               cPRO_clave,
	               cPRO_nombre,
	               regionRuta,
	               nombreregionruta,
	               ciudadRuta,
	               nombreciudadRuta,
	               cRUT_clave,
	               cRUT_nombre,
	               cTIR_clave,
	               cTIR_nombre,
	               cREG_clave,
	               cREG_nombre,
	               cCIU_clave,
	               cCIU_nombre,
	               cTIE_clave,
	               cTIE_nombre,
	               [1],
	               [2],
	               [3],
	               [4],
	               [5],
	               [6],
	               [0],
	               ClaveSupervisor,
	               Supervisor,
	               ClavePersonalRuta,
	               Personal,
	               CuentaCorreo,
	               nRUT_estatus,
	               dRUT_fechaactualizacion,
	               cRUT_actualizadopor,
	               cTIE_latitud,
	               cTIE_longitud,
	               nTIE_rangocheckin
	        FROM   (
	                   SELECT pc.cCLI_clave,
	                          pc.cCLI_nombre,
	                          pp.cPRO_clave,
	                          pp.cPRO_nombre,
	                          prr.cREG_clave AS regionRuta,
	                          prr.cREG_nombre AS nombreregionruta,
	                          bcr.cCIU_clave AS ciudadRuta,
	                          bcr.cCIU_nombre AS nombreciudadRuta,
	                          pr2.cRUT_clave,
	                          pr2.cRUT_nombre,
	                          pr2.cTIR_clave,
	                          ptr.cTIR_nombre,
	                          prppt.cREG_clave,
	                          pr.cREG_nombre,
	                          prppt.cCIU_clave,
	                          bc.cCIU_nombre,
	                          prppt.cTIE_clave,
	                          pt.cTIE_nombre,
	                          nDIP_clave,
	                          nRUT_ordenvisita,
	                          pR2.nPER_supervisor AS ClaveSupervisor,
	                          pp2.cPER_nombres + ' ' + pp2.cPER_apellidopaterno 
	                          + ' ' +
	                          ISNULL(pp2.cPER_apellidomaterno, '') AS Supervisor,
	                          prpp.nPER_clave AS ClavePersonalRuta,
	                          pp2t.cPER_nombres + ' ' + pp2t.cPER_apellidopaterno 
	                          + ' ' +
	                          ISNULL(pp2t.cPER_apellidomaterno, '') AS Personal,
	                          pp2t.cPER_cuentacorreo AS CuentaCorreo,
	                          CASE 
	                               WHEN pr2.nRUT_estatus = 'True' THEN 'Activo'
	                               ELSE 'Inactivo'
	                          END AS nRUT_estatus,
	                          pr2.dRUT_fechaactualizacion,
	                          pr2.cRUT_actualizadopor,
	                          pt.cTIE_latitud,
	                          pt.cTIE_longitud,
	                          pt.nTIE_rangocheckin
	                   FROM   PMC_Ruta_Programacion_PMC_Tienda AS prppt
	                          INNER JOIN PMC_Ruta AS pr2
	                               ON  pr2.cRUT_clave = prppt.cRUT_clave
	                               AND pr2.cPRO_clave = prppt.cPRO_clave
	                          INNER JOIN PMC_Promocion AS pp
	                               ON  pp.cPRO_clave = pr2.cPRO_clave
	                               AND pp.cPRO_clave = prppt.cPRO_clave
	                          INNER JOIN PMC_Cliente AS pc
	                               ON  pc.cCLI_clave = pp.cCLI_clave
	                               AND pc.nEPS_claveempresa = pp.nEPS_claveempresa
	                          INNER JOIN PMC_Tienda AS pt
	                               ON  pt.cPRO_clave = prppt.cPRO_clave
	                               AND pt.cTIE_clave = prppt.cTIE_clave
	                               AND pt.cCIU_clave = prppt.cCIU_clave
	                               AND pt.cREG_clave = prppt.cREG_clave
	                          INNER JOIN PMC_TipoRuta AS ptr
	                               ON  ptr.cTIR_clave = pr2.cTIR_clave
	                          INNER JOIN BO_Ciudad AS bc
	                               ON  bc.cCIU_clave = prppt.cCIU_clave
	                          INNER JOIN PMC_Region AS pr
	                               ON  pr.cPRO_clave = prppt.cPRO_clave
	                               AND pr.cREG_clave = prppt.cREG_clave
	                          LEFT OUTER JOIN BO_Ciudad AS bcr
	                               ON  bcr.cCIU_clave = pr2.cCIU_clave
	                          LEFT OUTER JOIN PMC_Region AS prr
	                               ON  prr.cPRO_clave = pr2.cPRO_clave
	                               AND prr.cREG_clave = pr2.cREG_clave
	                          LEFT OUTER JOIN PMC_Personal AS pp2
	                               ON  pp2.[nPER_clave] = pr2.nPER_supervisor
	                               AND pp2.nEPS_claveempresa = pc.nEPS_claveempresa
	                          LEFT OUTER JOIN PMC_Ruta_PMC_Personal AS prpp
	                               ON  (
	                                       prpp.dRUT_fechatermino = ''
	                                       OR prpp.dRUT_fechatermino IS NULL
	                                   )
	                               AND prpp.[cRUT_clave] = pr2.[cRUT_clave]
	                               AND prpp.[cPRO_clave] = pr2.[cPRO_clave]
	                               AND prpp.[cCLI_clave] = pc.[cCLI_clave]
	                               AND prpp.[nEPS_claveempresa] = pc.[nEPS_claveempresa]
	                          LEFT OUTER JOIN PMC_Personal AS pp2t
	                               ON  pp2t.nPER_clave = prpp.nPER_clave
	                               AND pp2t.nEPS_claveempresa = pc.nEPS_claveempresa
	                   WHERE  prppt.cPRO_clave = @cPRO_clave
	                          AND pc.cCLI_clave = @cCLI_clave
	                          AND pc.nEPS_claveempresa = 3
	                          AND prpp.nPER_clave = @nPER_clave
	                          AND pt.cTIE_clave NOT IN (SELECT t.[cTIE_clave]
	                                                    FROM   [dbo].[PMC_Ruta_Programacion] AS 
	                                                           rp
	                                                           INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS 
	                                                                rpt
	                                                                ON  rp.[cRUT_clave] = 
	                                                                    rpt.[cRUT_clave]
	                                                                AND rp.[cPRO_clave] = 
	                                                                    rpt.[cPRO_clave]
	                                                                AND rp.[nDIP_clave] = 
	                                                                    rpt.[nDIP_clave]
	                                                           INNER JOIN [dbo].[PMC_Tienda] AS 
	                                                                t
	                                                                ON  t.[cPRO_clave] = 
	                                                                    rp.[cPRO_clave]
	                                                                AND t.[cTIE_clave] = 
	                                                                    rpt.[cTIE_clave]
	                                                                AND t.[cCIU_clave] = 
	                                                                    rpt.[cCIU_clave]
	                                                                AND t.[cREG_clave] = 
	                                                                    rpt.[cREG_clave]
	                                                                AND t.[nTIE_estatus] = 
	                                                                    1
	                                                           LEFT OUTER JOIN 
	                                                                [dbo].[PMC_Region] AS 
	                                                                r
	                                                                ON  r.[cREG_clave] = 
	                                                                    t.[cREG_clave]
	                                                                AND r.[cPRO_clave] = 
	                                                                    t.[cPRO_clave]
	                                                           LEFT OUTER JOIN 
	                                                                [dbo].[BO_Ciudad] AS 
	                                                                c
	                                                                ON  c.[cCIU_clave] = 
	                                                                    t.[cCIU_clave]
	                                                    WHERE  rp.cRUT_clave = @cRUT_clave
	                                                           AND rp.cPRO_clave = 
	                                                               @cPRO_clave
	                                                           AND rp.nDIP_clave = 
	                                                               @nDIP_clave
	                                                           )
	               ) AS SourceTable 
	               PIVOT(
	                   AVG(nRUT_ordenvisita) 
	                   FOR nDIP_clave IN ([1], [2], [3], [4], [5], [6], [0])
	               ) AS PivotTable;
	    END

		-- Visited Stores

		IF (@TipoFueraRuta = 1) --Region-Ciudad
	    BEGIN
			SELECT t.[cTIE_clave],
	           t.[cTIE_nombre],
	           r.[cREG_clave],
	           r.[cREG_nombre],
	           c.[cCIU_clave],
	           c.[cCIU_nombre],
	           t.[cTIE_latitud],
	           t.[cTIE_longitud],
	           t.[nTIE_rangocheckin]
	    FROM   [dbo].[PMC_Ruta_Programacion] AS rp
	           INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS rpt
	                ON  rp.[cRUT_clave] = rpt.[cRUT_clave]
	                AND rp.[cPRO_clave] = rpt.[cPRO_clave]
	                AND rp.[nDIP_clave] = rpt.[nDIP_clave]
	           INNER JOIN [dbo].[PMC_Tienda] AS t
	                ON  t.[cPRO_clave] = rp.[cPRO_clave]
	                AND t.[cTIE_clave] = rpt.[cTIE_clave]
	                AND t.[cCIU_clave] = rpt.[cCIU_clave]
	                AND t.[cREG_clave] = rpt.[cREG_clave]
	                AND t.[nTIE_estatus] = 1
	           LEFT OUTER JOIN [dbo].[PMC_Region] AS r
	                ON  r.[cREG_clave] = t.[cREG_clave]
	                AND r.[cPRO_clave] = t.[cPRO_clave]
	           LEFT OUTER JOIN [dbo].[BO_Ciudad] AS c
	                ON  c.[cCIU_clave] = t.[cCIU_clave]
	    WHERE  rp.cRUT_clave = @cRUT_clave
	           AND rp.cPRO_clave = @cPRO_clave
	           AND rp.nDIP_clave = @nDIP_clave
	           AND t.[cTIE_clave] IN (SELECT [cTIE_clave]
	                                      FROM   [dbo].[POD_Sincronizacion]
	                                      WHERE  cPRO_clave = @cPRO_clave
	                                             AND cRUT_clave = @cRUT_clave
	                                             AND rp.nDIP_clave = @nDIP_clave
	                                             AND CONVERT(DATE, [dSIN_fechainicio]) = 
	                                                 CONVERT(DATE, GETDATE()))
	   

				UNION
	        --TODO: GET ALL THE ROUTE'S STOTES OF THE PROMO,  EXCEPT THE STORES THAT BELONG TO THE SCHEDULED ROUTE FOR THE CURRENT DAY
	        SELECT t.[cTIE_clave],
	               t.[cTIE_nombre],
	               r.[cREG_clave],
	               r.[cREG_nombre],
	               c.[cCIU_clave],
	               c.[cCIU_nombre],
	               t.[cTIE_latitud],
	               t.[cTIE_longitud],
	               t.[nTIE_rangocheckin]
	        FROM   [dbo].[PMC_Ruta_BO_Ciudad_PMC_Region] AS rcr
	               INNER JOIN [dbo].[PMC_Tienda] AS t
	                    ON  t.[cPRO_clave] = rcr.[cPRO_clave]
	                    AND t.[cREG_clave] = rcr.[cREG_clave]
	                    AND t.[cCIU_clave] = rcr.[cCIU_clave]
	               INNER JOIN [dbo].[PMC_Region] AS r
	                    ON  r.[cREG_clave] = rcr.[cREG_clave]
	                    AND r.[cPRO_clave] = rcr.[cPRO_clave]
	               INNER JOIN [dbo].[BO_Ciudad] AS c
	                    ON  c.[cCIU_clave] = rcr.[cCIU_clave]
	        WHERE  rcr.cRUT_clave = @cRUT_clave
	               AND rcr.cPRO_clave = @cPRO_clave
				   AND t.[cTIE_clave] IN (SELECT [cTIE_clave]
	                                      FROM   [dbo].[POD_Sincronizacion]
	                                      WHERE  cPRO_clave = @cPRO_clave
	                                             AND cRUT_clave = @cRUT_clave
	                                             AND nDIP_clave = @nDIP_clave
	                                             AND CONVERT(DATE, [dSIN_fechainicio]) = 
	                                                 CONVERT(DATE, GETDATE()))
	               AND t.cTIE_clave NOT IN (SELECT t.[cTIE_clave]
	                                        FROM   [dbo].[PMC_Ruta_Programacion] AS 
	                                               rp
	                                               INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS 
	                                                    rpt
	                                                    ON  rp.[cRUT_clave] = 
	                                                        rpt.[cRUT_clave]
	                                                    AND rp.[cPRO_clave] = 
	                                                        rpt.[cPRO_clave]
	                                                    AND rp.[nDIP_clave] = 
	                                                        rpt.[nDIP_clave]
	                                               INNER JOIN [dbo].[PMC_Tienda] AS 
	                                                    t
	                                                    ON  t.[cPRO_clave] = rp.[cPRO_clave]
	                                                    AND t.[cTIE_clave] = rpt.[cTIE_clave]
	                                                    AND t.[cCIU_clave] = rpt.[cCIU_clave]
	                                                    AND t.[cREG_clave] = rpt.[cREG_clave]
	                                                    AND t.[nTIE_estatus] = 1
	                                        WHERE  rp.cRUT_clave = @cRUT_clave
	                                               AND rp.cPRO_clave = @cPRO_clave
	                                               AND rp.nDIP_clave = @nDIP_clave)
	        ORDER BY
	               t.[cTIE_nombre]

	    END
	    ELSE
	        --Programacion
	    BEGIN
	    	SELECT t.[cTIE_clave],
	           t.[cTIE_nombre],
	           r.[cREG_clave],
	           r.[cREG_nombre],
	           c.[cCIU_clave],
	           c.[cCIU_nombre],
	           t.[cTIE_latitud],
	           t.[cTIE_longitud],
	           t.[nTIE_rangocheckin]
	    FROM   [dbo].[PMC_Ruta_Programacion] AS rp
	           INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS rpt
	                ON  rp.[cRUT_clave] = rpt.[cRUT_clave]
	                AND rp.[cPRO_clave] = rpt.[cPRO_clave]
	                AND rp.[nDIP_clave] = rpt.[nDIP_clave]
	           INNER JOIN [dbo].[PMC_Tienda] AS t
	                ON  t.[cPRO_clave] = rp.[cPRO_clave]
	                AND t.[cTIE_clave] = rpt.[cTIE_clave]
	                AND t.[cCIU_clave] = rpt.[cCIU_clave]
	                AND t.[cREG_clave] = rpt.[cREG_clave]
	                AND t.[nTIE_estatus] = 1
	           LEFT OUTER JOIN [dbo].[PMC_Region] AS r
	                ON  r.[cREG_clave] = t.[cREG_clave]
	                AND r.[cPRO_clave] = t.[cPRO_clave]
	           LEFT OUTER JOIN [dbo].[BO_Ciudad] AS c
	                ON  c.[cCIU_clave] = t.[cCIU_clave]
	    WHERE  rp.cRUT_clave = @cRUT_clave
	           AND rp.cPRO_clave = @cPRO_clave
	           AND rp.nDIP_clave = @nDIP_clave
	           AND t.[cTIE_clave] IN (SELECT [cTIE_clave]
	                                      FROM   [dbo].[POD_Sincronizacion]
	                                      WHERE  cPRO_clave = @cPRO_clave
	                                             AND cRUT_clave = @cRUT_clave
	                                             AND rp.nDIP_clave = @nDIP_clave
	                                             AND CONVERT(DATE, [dSIN_fechainicio]) = 
	                                                 CONVERT(DATE, GETDATE()))
	   

				UNION
	        SELECT cTIE_clave,
	               cTIE_nombre,
	               cREG_clave,
	               cREG_nombre,
	               cCIU_clave,
	               cCIU_nombre,
	               --cCLI_clave,
	               --cCLI_nombre,
	               --cPRO_clave,
	               --cPRO_nombre,
	               --regionRuta,
	               --nombreregionruta,
	               --ciudadRuta,
	               --nombreciudadRuta,
	               --cRUT_clave,
	               --cRUT_nombre,
	               --cTIR_clave,
	               --cTIR_nombre,
	               cTIE_latitud,
	               cTIE_longitud,
	               nTIE_rangocheckin
	               --[1],
	               --[2],
	               --[3],
	               --[4],
	               --[5],
	               --[6],
	               --[0],
	               --ClaveSupervisor,
	               --Supervisor,
	               --ClavePersonalRuta,
	               --Personal,
	               --CuentaCorreo,
	               --nRUT_estatus,
	               --dRUT_fechaactualizacion,
	               --cRUT_actualizadopor
	               
	        FROM   (
	                   SELECT pc.cCLI_clave,
	                          pc.cCLI_nombre,
	                          pp.cPRO_clave,
	                          pp.cPRO_nombre,
	                          prr.cREG_clave AS regionRuta,
	                          prr.cREG_nombre AS nombreregionruta,
	                          bcr.cCIU_clave AS ciudadRuta,
	                          bcr.cCIU_nombre AS nombreciudadRuta,
	                          pr2.cRUT_clave,
	                          pr2.cRUT_nombre,
	                          pr2.cTIR_clave,
	                          ptr.cTIR_nombre,
	                          prppt.cREG_clave,
	                          pr.cREG_nombre,
	                          prppt.cCIU_clave,
	                          bc.cCIU_nombre,
	                          prppt.cTIE_clave,
	                          pt.cTIE_nombre,
	                          nDIP_clave,
	                          nRUT_ordenvisita,
	                          pR2.nPER_supervisor AS ClaveSupervisor,
	                          pp2.cPER_nombres + ' ' + pp2.cPER_apellidopaterno 
	                          + ' ' +
	                          ISNULL(pp2.cPER_apellidomaterno, '') AS Supervisor,
	                          prpp.nPER_clave AS ClavePersonalRuta,
	                          pp2t.cPER_nombres + ' ' + pp2t.cPER_apellidopaterno 
	                          + ' ' +
	                          ISNULL(pp2t.cPER_apellidomaterno, '') AS Personal,
	                          pp2t.cPER_cuentacorreo AS CuentaCorreo,
	                          CASE 
	                               WHEN pr2.nRUT_estatus = 'True' THEN 'Activo'
	                               ELSE 'Inactivo'
	                          END AS nRUT_estatus,
	                          pr2.dRUT_fechaactualizacion,
	                          pr2.cRUT_actualizadopor,
	                          pt.cTIE_latitud,
	                          pt.cTIE_longitud,
	                          pt.nTIE_rangocheckin
	                   FROM   PMC_Ruta_Programacion_PMC_Tienda AS prppt
	                          INNER JOIN PMC_Ruta AS pr2
	                               ON  pr2.cRUT_clave = prppt.cRUT_clave
	                               AND pr2.cPRO_clave = prppt.cPRO_clave
	                          INNER JOIN PMC_Promocion AS pp
	                               ON  pp.cPRO_clave = pr2.cPRO_clave
	                               AND pp.cPRO_clave = prppt.cPRO_clave
	                          INNER JOIN PMC_Cliente AS pc
	                               ON  pc.cCLI_clave = pp.cCLI_clave
	                               AND pc.nEPS_claveempresa = pp.nEPS_claveempresa
	                          INNER JOIN PMC_Tienda AS pt
	                               ON  pt.cPRO_clave = prppt.cPRO_clave
	                               AND pt.cTIE_clave = prppt.cTIE_clave
	                               AND pt.cCIU_clave = prppt.cCIU_clave
	                               AND pt.cREG_clave = prppt.cREG_clave
	                          INNER JOIN PMC_TipoRuta AS ptr
	                               ON  ptr.cTIR_clave = pr2.cTIR_clave
	                          INNER JOIN BO_Ciudad AS bc
	                               ON  bc.cCIU_clave = prppt.cCIU_clave
	                          INNER JOIN PMC_Region AS pr
	                               ON  pr.cPRO_clave = prppt.cPRO_clave
	                               AND pr.cREG_clave = prppt.cREG_clave
	                          LEFT OUTER JOIN BO_Ciudad AS bcr
	                               ON  bcr.cCIU_clave = pr2.cCIU_clave
	                          LEFT OUTER JOIN PMC_Region AS prr
	                               ON  prr.cPRO_clave = pr2.cPRO_clave
	                               AND prr.cREG_clave = pr2.cREG_clave
	                          LEFT OUTER JOIN PMC_Personal AS pp2
	                               ON  pp2.[nPER_clave] = pr2.nPER_supervisor
	                               AND pp2.nEPS_claveempresa = pc.nEPS_claveempresa
	                          LEFT OUTER JOIN PMC_Ruta_PMC_Personal AS prpp
	                               ON  (
	                                       prpp.dRUT_fechatermino = ''
	                                       OR prpp.dRUT_fechatermino IS NULL
	                                   )
	                               AND prpp.[cRUT_clave] = pr2.[cRUT_clave]
	                               AND prpp.[cPRO_clave] = pr2.[cPRO_clave]
	                               AND prpp.[cCLI_clave] = pc.[cCLI_clave]
	                               AND prpp.[nEPS_claveempresa] = pc.[nEPS_claveempresa]
	                          LEFT OUTER JOIN PMC_Personal AS pp2t
	                               ON  pp2t.nPER_clave = prpp.nPER_clave
	                               AND pp2t.nEPS_claveempresa = pc.nEPS_claveempresa
	                   WHERE  prppt.cPRO_clave = @cPRO_clave
	                          AND pc.cCLI_clave = @cCLI_clave
	                          AND pc.nEPS_claveempresa = 3
	                          AND prpp.nPER_clave = @nPER_clave
							  AND pt.[cTIE_clave] IN (SELECT [cTIE_clave]
	                                      FROM   [dbo].[POD_Sincronizacion]
	                                      WHERE  cPRO_clave = @cPRO_clave
	                                             AND cRUT_clave = @cRUT_clave
	                                             AND nDIP_clave = @nDIP_clave
	                                             AND CONVERT(DATE, [dSIN_fechainicio]) = 
	                                                 CONVERT(DATE, GETDATE()))
	                          AND pt.cTIE_clave NOT IN (SELECT t.[cTIE_clave]
	                                                    FROM   [dbo].[PMC_Ruta_Programacion] AS 
	                                                           rp
	                                                           INNER JOIN [dbo].[PMC_Ruta_Programacion_PMC_Tienda] AS 
	                                                                rpt
	                                                                ON  rp.[cRUT_clave] = 
	                                                                    rpt.[cRUT_clave]
	                                                                AND rp.[cPRO_clave] = 
	                                                                    rpt.[cPRO_clave]
	                                                                AND rp.[nDIP_clave] = 
	                                                                    rpt.[nDIP_clave]
	                                                           INNER JOIN [dbo].[PMC_Tienda] AS 
	                                                                t
	                                                                ON  t.[cPRO_clave] = 
	                                                                    rp.[cPRO_clave]
	                                                                AND t.[cTIE_clave] = 
	                                                                    rpt.[cTIE_clave]
	                                                                AND t.[cCIU_clave] = 
	                                                                    rpt.[cCIU_clave]
	                                                                AND t.[cREG_clave] = 
	                                                                    rpt.[cREG_clave]
	                                                                AND t.[nTIE_estatus] = 
	                                                                    1
	                                                           LEFT OUTER JOIN 
	                                                                [dbo].[PMC_Region] AS 
	                                                                r
	                                                                ON  r.[cREG_clave] = 
	                                                                    t.[cREG_clave]
	                                                                AND r.[cPRO_clave] = 
	                                                                    t.[cPRO_clave]
	                                                           LEFT OUTER JOIN 
	                                                                [dbo].[BO_Ciudad] AS 
	                                                                c
	                                                                ON  c.[cCIU_clave] = 
	                                                                    t.[cCIU_clave]
	                                                    WHERE  rp.cRUT_clave = @cRUT_clave
	                                                           AND rp.cPRO_clave = 
	                                                               @cPRO_clave
	                                                           AND rp.nDIP_clave = 
	                                                               @nDIP_clave
	                                                           )
	               ) AS SourceTable 
	               PIVOT(
	                   AVG(nRUT_ordenvisita) 
	                   FOR nDIP_clave IN ([1], [2], [3], [4], [5], [6], [0])
	               ) AS PivotTable;
	    END
	END
	ELSE 
	IF (@Action = 'MensajeRutaDelDia')
	BEGIN
	    SET @LocalnPER_clave = (
	            SELECT TOP 1 [nPER_clave]
	            FROM   [dbo].[PMC_Personal]
	            WHERE  [cPER_cuentacorreo] = @cPER_cuentacorreo
	        );
	    
	    SET @cRUT_clave = (
	            SELECT TOP 1
	                   rp.cRUT_clave
	            FROM   [dbo].[PMC_Ruta_PMC_Personal] AS rp
	                   INNER JOIN [dbo].[PMC_Ruta] AS r
	                        ON  r.[cRUT_clave] = rp.[cRUT_clave]
	                        AND r.[cPRO_clave] = rp.[cPRO_clave]
	                   INNER JOIN [dbo].[PMC_Promocion] AS p
	                        ON  p.[cPRO_clave] = rp.[cPRO_clave]
	                        AND p.[nEPS_claveempresa] = rp.[nEPS_claveempresa]
	                        AND p.[cCLI_clave] = rp.[cCLI_clave]
	            WHERE  rp.[cPRO_clave] = @cPRO_clave
	                   AND rp.nPER_clave = @LocalnPER_clave
	                   AND r.[nRUT_estatus] = 1
	                   AND (rp.dRUT_fechatermino = '' OR rp.dRUT_fechatermino IS NULL)
	            ORDER BY
	                   r.[cRUT_nombre]
	        );
	    
	    IF EXISTS(
	           SELECT [cTIE_clave]
	           FROM   [dbo].[POD_Sincronizacion]
	           WHERE  cPRO_clave         = @cPRO_clave
	                  AND cRUT_clave     = @cRUT_clave
	                  AND nDIP_clave     = @nDIP_clave
	                  AND CONVERT(DATE, [dSIN_fechainicio]) = CONVERT(DATE, GETDATE())
	       )
	    BEGIN
	        SELECT [cVAR_clave],
	               [cVAR_valor]
	        FROM   [dbo].[APP_Variables]
	        WHERE  [cVAR_clave] = 'WSTextoRutaConcluida'
	    END
	    ELSE
	    BEGIN
	        SELECT [cVAR_clave],
	               [cVAR_valor]
	        FROM   [dbo].[APP_Variables]
	        WHERE  [cVAR_clave] = 'WSTestoSinTienda'
	    END
	END
	ELSE 
	
	IF (@Action = 'SaveMotiveNoVisit')
	BEGIN
	    --SET @LocalnPER_clave = (
	    --        SELECT TOP 1 [nPER_clave]
	    --        FROM   [dbo].[PMC_Personal]
	    --        WHERE  [cPER_cuentacorreo] = @cPER_cuentacorreo
	    --    );
	    
	    --SELECT @cCIU_clave = pt.cCIU_clave,
	    --       @cREG_clave = pt.cREG_clave
	    --FROM   PMC_Tienda AS pt
	    --WHERE  pt.cTIE_clave = @cTIE_clave
	    --       AND pt.cPRO_clave = @cPRO_clave;
	    --IF NOT EXISTS (
	    --       SELECT pppmnv.cPRO_clave,
	    --              pppmnv.cMNV_clave
	    --       FROM   PMC_Promocion_PMC_MotivosNoVisita AS pppmnv
	    --       WHERE  pppmnv.cPRO_clave = @cPRO_clave
	    --              AND pppmnv.cMNV_clave = @cMNV_clave
	    --   )
	    --BEGIN
	    --    INSERT INTO PMC_Promocion_PMC_MotivosNoVisita
	    --      (
	    --        cPRO_clave,
	    --        cMNV_clave
	    --      )
	    --    VALUES
	    --      (
	    --        @cPRO_clave,
	    --        @cMNV_clave
	    --      )
	    --END
	    
	    --INSERT INTO POD_Sincronizacion
	    --  (
	    --    nSIN_clave,
	    --    cRUT_clave,
	    --    cPRO_clave,
	    --    nDIP_clave,
	    --    cREG_clave,
	    --    cCIU_clave,
	    --    cTIE_clave,
	    --    nPER_clave,
	    --    --dSIN_fechainicio,
	    --    --dSIN_horainicio,
	    --    dSIN_fechatermino,
	    --    dSIN_horatermino,
	    --    cMNV_clave --MOTIVO NO VISITA
	    --  )
	    --VALUES
	    --  (
	    --    @nSIN_clave,
	    --    @cRUT_clave,
	    --    @cPRO_clave,
	    --    @nDIP_clave,
	    --    @cREG_clave,
	    --    @cCIU_clave,
	    --    @cTIE_clave,
	    --    @LocalnPER_clave,
	    --    CONVERT(SMALLDATETIME, GETDATE()),
	    --    CONVERT(TIME, GETDATE()),
	    --    @cMNV_clave
	    --  )
	    
	    SET @LocalnPER_clave = (
	            SELECT TOP 1 [nPER_clave]
	            FROM   [dbo].[PMC_Personal]
	            WHERE  [cPER_cuentacorreo] = @cPER_cuentacorreo
	        );
	    
	    SELECT @cCIU_clave = pt.cCIU_clave,
	           @cREG_clave = pt.cREG_clave
	    FROM   PMC_Tienda AS pt
	    WHERE  pt.cTIE_clave = @cTIE_clave
	           AND pt.cPRO_clave = @cPRO_clave;
	    
	    INSERT INTO POD_Sincronizacion
	      (
	        nSIN_clave,
	        cRUT_clave,
	        cPRO_clave,
	        nDIP_clave,
	        cREG_clave,
	        cCIU_clave,
	        cTIE_clave,
	        nPER_clave,
	        dSIN_fechainicio,
	        dSIN_horainicio,
	        dSIN_fechatermino,
	        dSIN_horatermino,
	        cMNV_clave --MOTIVO NO VISITA
	      )
	    VALUES
	      (
	        @nSIN_clave,
	        @cRUT_clave,
	        @cPRO_clave,
	        @nDIP_clave,
	        @cREG_clave,
	        @cCIU_clave,
	        @cTIE_clave,
	        @LocalnPER_clave,
	        CONVERT(SMALLDATETIME, GETDATE()),
	        CONVERT(TIME, GETDATE()),
	        CONVERT(SMALLDATETIME, GETDATE()),
	        CONVERT(TIME, GETDATE()),
	        @cMNV_clave
	      )
	END
	ELSE 
	IF (@Action = 'SaveCheckinData')
	BEGIN
	    SET @LocalnPER_clave = (
	            SELECT TOP 1 [nPER_clave]
	            FROM   [dbo].[PMC_Personal]
	            WHERE  [cPER_cuentacorreo] = @cPER_cuentacorreo
	        );
	    
	    SELECT @cCIU_clave = pt.cCIU_clave,
	           @cREG_clave = pt.cREG_clave
	    FROM   PMC_Tienda AS pt
	    WHERE  pt.cTIE_clave = @cTIE_clave
	           AND pt.cPRO_clave = @cPRO_clave;
	    INSERT INTO POD_Sincronizacion
	      (
	        nSIN_clave,
	        cRUT_clave,
	        cPRO_clave,
	        nDIP_clave,
	        cREG_clave,
	        cCIU_clave,
	        cTIE_clave,
	        nPER_clave,
	        dSIN_fechainicio,
	        dSIN_horainicio,
	        --dSIN_fechatermino,
	        --dSIN_horatermino,
	        cMNV_clave --MOTIVO NO VISITA
	      )
	    VALUES
	      (
	        @nSIN_clave,
	        @cRUT_clave,
	        @cPRO_clave,
	        @nDIP_clave,
	        @cREG_clave,
	        @cCIU_clave,
	        @cTIE_clave,
	        @LocalnPER_clave,
	        CONVERT(SMALLDATETIME, GETDATE()),
	        CONVERT(TIME, GETDATE()),
	        @cMNV_clave
	      )
	    INSERT INTO POD_CheckinCheckout
	      (
	        nSIN_clave,
	        cRUT_clave,
	        cPRO_clave,
	        nDIP_clave,
	        cREG_clave,
	        cCIU_clave,
	        cTIE_clave,
	        bSIN_checkin,
	        cSIN_checkinlatitud,
	        cSIN_checkinlongitud,
	        dSIN_checkinfecha,
	        dSIN_checkinhora
	      )
	    VALUES
	      (
	        @nSIN_clave,
	        @cRUT_clave,
	        @cPRO_clave,
	        @nDIP_clave,
	        @cREG_clave,
	        @cCIU_clave,
	        @cTIE_clave,
	        @bSIN_checkin,
	        REPLACE(@cSIN_checkinlatitud, ',', '.'),
	        REPLACE(@cSIN_checkinlongitud, ',', '.'),
	        CONVERT(SMALLDATETIME, @dSIN_checkinfecha),
	        CONVERT(TIME, @dSIN_checkinhora)
	      )
	END
	
	IF (@Action = 'SaveCheckoutData')
	BEGIN
	    UPDATE POD_CheckinCheckout
	    SET    nSIN_clave = @nSIN_clave,
	           bSIN_checkout = @bSIN_checkout,
	           cSIN_checkoutlatitud = REPLACE(@cSIN_checkoutlatitud, ',', '.'),
	           cSIN_checkoutlongitud = REPLACE(@cSIN_checkoutlongitud, ',', '.'),
	           dSIN_checkoutfecha = CONVERT(SMALLDATETIME, @dSIN_checkoutfecha),
	           dSIN_checkouthora = CONVERT(TIME, @dSIN_checkouthora),
	           cSIN_checkoutobservaciones = @cSIN_checkoutobservaciones,
	           dSIN_fechaactualizacion = GETDATE()
	    WHERE  nSIN_clave = @nSIN_clave
	    
	    UPDATE POD_Sincronizacion
	    SET    dSIN_fechatermino           = CONVERT(SMALLDATETIME, GETDATE()),
	           dSIN_horatermino            = CONVERT(TIME, GETDATE()),
	           dSIN_fechaactualizacion     = GETDATE()
	    WHERE  nSIN_clave                  = @nSIN_clave
	END
	
	IF (@Action = 'SaveSurveyData')
	BEGIN
	    DECLARE @cntEncuestas INT;
	    SELECT @cntEncuestas = COUNT(nSIN_clave)
	    FROM   POD_Encuestas AS ps
	    WHERE  ps.nSIN_clave = @nSIN_clave
	           AND ps.cTIE_clave = @cTIE_clave
	    
	    IF @cntEncuestas = 0
	    BEGIN
	        INSERT INTO POD_Encuestas
	          (
	            nSIN_clave,
	            cRUT_clave,
	            cPRO_clave,
	            nDIP_clave,
	            cREG_clave,
	            cCIU_clave,
	            cTIE_clave,
	            cENC_clave,
	            bENC_respuesta1,
	            bENC_respuesta2,
	            bENC_respuesta3,
	            bENC_respuesta4,
	            bENC_respuesta5,
	            bENC_respuesta6,
	            bENC_respuesta7,
	            bENC_respuesta8,
	            bENC_respuesta9,
	            bENC_respuesta10,
	            bENC_respuesta11,
	            bENC_respuesta12,
	            bENC_respuesta13,
	            bENC_respuesta14,
	            bENC_respuesta15,
	            cENC_comentarios,
	            dENC_horainicio,
	            dENC_horatermino,
	            dENC_fechasincronizacion,
	            dENC_horasincronizacion
	          )
	        SELECT ps.nSIN_clave,
	               ps.cRUT_clave,
	               ps.cPRO_clave,
	               ps.nDIP_clave,
	               ps.cREG_clave,
	               ps.cCIU_clave,
	               ps.cTIE_clave,
	               @cENC_clave,
	               @bENC_respuesta1,
	               @bENC_respuesta2,
	               @bENC_respuesta3,
	               @bENC_respuesta4,
	               @bENC_respuesta5,
	               @bENC_respuesta6,
	               @bENC_respuesta7,
	               @bENC_respuesta8,
	               @bENC_respuesta9,
	               @bENC_respuesta10,
	               @bENC_respuesta11,
	               @bENC_respuesta12,
	               @bENC_respuesta13,
	               @bENC_respuesta14,
	               @bENC_respuesta15,
	               @cENC_comentarios,
	               CONVERT(SMALLDATETIME, @dENC_horainicio),
	               CONVERT(SMALLDATETIME, @dENC_horatermino),
	               CONVERT(SMALLDATETIME, GETDATE()),
	               CONVERT(SMALLDATETIME, GETDATE())
	        FROM   POD_Sincronizacion AS ps
	        WHERE  ps.nSIN_clave = @nSIN_clave
	               AND ps.cTIE_clave = @cTIE_clave
	    END
	    ELSE
	    BEGIN
	        UPDATE POD_Encuestas
	        SET    bENC_respuesta1      = @bENC_respuesta1,
	               bENC_respuesta2      = @bENC_respuesta2,
	               bENC_respuesta3      = @bENC_respuesta3,
	               bENC_respuesta4      = @bENC_respuesta4,
	               bENC_respuesta5      = @bENC_respuesta5,
	               bENC_respuesta6      = @bENC_respuesta6,
	               bENC_respuesta7      = @bENC_respuesta7,
	               bENC_respuesta8      = @bENC_respuesta8,
	               bENC_respuesta9      = @bENC_respuesta9,
	               bENC_respuesta10     = @bENC_respuesta10,
	               bENC_respuesta11     = @bENC_respuesta11,
	               bENC_respuesta12     = @bENC_respuesta12,
	               bENC_respuesta13     = @bENC_respuesta13,
	               bENC_respuesta14     = @bENC_respuesta14,
	               bENC_respuesta15     = @bENC_respuesta15,
	               cENC_comentarios     = @cENC_comentarios,
	               dENC_horainicio      = CONVERT(SMALLDATETIME, @dENC_horainicio),
	               dENC_horatermino     = CONVERT(SMALLDATETIME, @dENC_horatermino),
	               dENC_fechasincronizacion = CONVERT(SMALLDATETIME, GETDATE()),
	               dENC_horasincronizacion = CONVERT(SMALLDATETIME, GETDATE()),
	               dENC_fechaactualizacion = GETDATE()
	        WHERE  nSIN_clave           = @nSIN_clave
	               AND cTIE_clave       = @cTIE_clave
	    END
	END
	ELSE 
	IF (@Action = 'ValidateEmail')
	BEGIN
	    SELECT pp.nPER_clave
	    FROM   PMC_Personal AS pp
	    WHERE  pp.cPER_cuentacorreo = @cPER_cuentacorreo
	END
	ELSE 
	IF (@Action = 'UpdateSendPassword')
	BEGIN
	    BEGIN TRY
	    	BEGIN TRANSACTION
	    	
	    	OPEN SYMMETRIC KEY SymKey_PWD
	    	DECRYPTION BY CERTIFICATE PWD_Certificate;
	    	
	    	UPDATE PMC_Personal
	    	SET    cPER_contrasena = ENCRYPTBYKEY(KEY_GUID('SymKey_PWD'), @cPER_contrasena),
	    	       dPER_fechaactualizacion = GETDATE(),
	    	       cPER_actualizadopor = 'System'
	    	WHERE  cPER_cuentacorreo = @cPER_cuentacorreo
	    	
	    	CLOSE SYMMETRIC KEY SymKey_PWD;
	    	
	    	SET @cPER_nombres = (
	    	        SELECT TOP 1
	    	               pp.cPER_nombres
	    	        FROM   PMC_Personal AS pp
	    	        WHERE  pp.cPER_cuentacorreo = @cPER_cuentacorreo
	    	    );
	    	
	    	SET @ProfileName = (
	    	        SELECT av.cVAR_valor
	    	        FROM   APP_Variables AS av
	    	        WHERE  av.cVAR_clave = 'ProfileName'
	    	    );
	    	
	    	SET @BodyEmailReset = (
	    	        SELECT REPLACE(
	    	                   REPLACE(av.cVAR_valor, '{0}', @cPER_nombres),
	    	                   '{1}',
	    	                   @cPER_contrasena
	    	               )
	    	        FROM   APP_Variables AS av
	    	        WHERE  av.cVAR_clave = 'WSTextoResetPassword'
	    	    );
	    	
	    	EXEC msdb.dbo.sp_send_dbmail 
	    	     @profile_name = @ProfileName,
	    	     @recipients = @cPER_cuentacorreo,
	    	     @subject = 'Reseteo de Contraseña',
	    	     @body_format = 'HTML',
	    	     @body = @BodyEmailReset,
	    	     @blind_copy_recipients = ''
	    	
	    	COMMIT
	    END TRY
	    BEGIN CATCH
	    	ROLLBACK
	    END CATCH
	END
	ELSE 
	IF (@Action = 'ValidateidSync')
	BEGIN
		 SET @LocalnPER_clave = (
	            SELECT TOP 1 [nPER_clave]
	            FROM   [dbo].[PMC_Personal]
	            WHERE  [cPER_cuentacorreo] = @cPER_cuentacorreo
	        );
	        
	    SELECT
	    	ps.nSIN_clave
	    FROM
	    	POD_Sincronizacion AS ps
	    WHERE ps.nSIN_clave= @nSIN_clave AND ps.nPER_clave = @LocalnPER_clave
	END
END