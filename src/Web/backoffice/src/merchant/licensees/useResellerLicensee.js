import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    mapChecked,
    mapCheckedById,
    mapCheckedByCultureCode,
    mapCheckedByCode,
    mapMergeChecked,
    mapCheckedByProp
} from "../../checkbox/mapChecked"
import { useReseller } from "../resellers/client"
import { useLicensee } from "./client"

export const useResellerLicensee = ({ id = "", resellerId = "" }) => {
    let {
        data: licensee,
        isLoading: isLoadingLicensee,
        error: licenseeError,
        mutate: mutateLicensee,
        deleteCache: deleteLicenseeCache
    } = useLicensee(id)
    let {
        data: reseller,
        isLoading: isLoadingReseller,
        error: resellerError
    } = useReseller(resellerId)

    let languages = mapCheckedByCultureCode(
        licensee?.languages ?? [],
        reseller?.languages ?? []
    )
    let currencies = mapCheckedByCode(
        licensee?.currencies ?? [],
        reseller?.currencies ?? []
    )
    let gameProviders = mapCheckedByProp(
        "gameProviderId",
        licensee?.gameProviders ?? [],
        reseller?.gameProviders ?? []
    )

    let betLimits = mapCheckedById(
        licensee?.betLimits,
        reseller?.betLimits ?? []
    )

    return {
        reseller,
        licensee,
        languages,
        currencies,
        gameProviders,
        betLimits,
        isLoading: isLoadingLicensee || isLoadingReseller,
        error: licenseeError || resellerError,
        mutateLicensee,
        deleteLicenseeCache
    }
}

export default useResellerLicensee
