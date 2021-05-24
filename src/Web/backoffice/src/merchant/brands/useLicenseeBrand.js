import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    mapChecked,
    mapCheckedById,
    mapCheckedByCultureCode,
    mapCheckedByCode,
    mapMergeChecked
} from "../../checkbox/mapChecked"
import { useBrand } from "./client"
import { useLicensee } from "../licensees/client"

export const useLicenseeBrand = ({ brandId = "", licenseeId = "" }) => {
    let {
        data: brand,
        isLoading: isLoadingBrand,
        error: brandError,
        mutate: mutateBrand,
        deleteCache: deleteBrandCache
    } = useBrand(brandId)

    let {
        data: licensee,
        isLoading: isLoadingLicensee,
        error: licenseeError,
        mutate: mutateLicensee,
        deleteCache: deleteLicenseeCache
    } = useLicensee(licenseeId)

    let gameProviders = mapMergeChecked(
        (brandGameProvider, licenseeGameProvider) => {
            return (
                brandGameProvider.gameProviderId ===
                licenseeGameProvider.gameProviderId
            )
        },
        (
            brandGameProvider = {
                configurationId: "",
                defaultLanguageCode: "",
                desktopQueryString: "",
                mobileQueryString: ""
            },
            licenseeGameProvider = {}
        ) => {
            return {
                ...licenseeGameProvider,
                ...brandGameProvider
            }
        },
        brand?.gameProviders ?? [],
        licensee?.gameProviders ?? []
    )

    let betLimits = mapMergeChecked(
        (brandBetLimit, betLimit) => {
            return brandBetLimit.id === betLimit.id
        },
        (brandBetLimit = { isDefault: false }, betLimit) => {
            return {
                ...betLimit,
                ...brandBetLimit
            }
        },
        brand?.betLimits ?? [],
        licensee?.betLimits ?? []
    )

    //TODO: rm games
    let games =
        brand?.games.map((game) => ({
            ...game,
            checked: true
        })) ?? []

    let languages = mapMergeChecked(
        (brandLang, licenseeLang) => {
            return brandLang.cultureCode === licenseeLang.cultureCode
        },
        (brandLang = {}, licenseeLang = {}) => {
            let brandCultureCode =
                brandLang.brandCultureCode ?? licenseeLang.cultureCode
            return {
                ...licenseeLang,
                ...brandLang,
                brandCultureCode
            }
        },
        brand?.languages ?? [],
        licensee?.languages ?? []
    )

    return {
        licensee,
        brand,
        businessType: brand?.businessType ?? licensee?.businessType,
        languages,
        currencies: mapCheckedByCode(
            brand?.currencies ?? [],
            licensee?.currencies ?? []
        ),
        gameProviders,
        games,
        betLimits,
        isLoading: isLoadingBrand || isLoadingLicensee,
        error: brandError || licenseeError,
        mutateBrand,
        deleteBrandCache
    }
}

export default useLicenseeBrand
