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

export const useLicenseeBrand = ({ id = "", licenseeId = "" }) => {
  let {
    data: brand,
    isLoading: isLoadingBrand,
    error: brandError,
    mutate: mutateBrand,
    deleteCache: deleteBrandCache
  } = useBrand(id)

  let {
    data: licensee,
    isLoading: isLoadingLicensee,
    error: licenseeError,
    mutate: mutateLicensee,
    deleteCache: deleteLicenseeCache
  } = useLicensee(licenseeId)

  let gameProviders = mapMergeChecked(
    (brandGameProvider, licenseeGameProvider) => {
      return brandGameProvider.gameProviderId === licenseeGameProvider.id
    },
    (brandGameProvider = {}, licenseeGameProvider = {}) => {
      return {
        ...brandGameProvider
      }
    },
    brand?.gameProviders ?? [],
    licensee?.gameProviders ?? []
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
      return {
        ...brandLang
      }
    },
    brand?.languages ?? [],
    licensee?.languages ?? []
  )

  return {
    licensee,
    brand,
    languages,
    currencies: mapCheckedByCode(
      brand?.currencies ?? [],
      licensee?.currencies ?? []
    ),
    gameProviders,
    games,
    betLimits: mapCheckedById(
      brand?.betLimits ?? [],
      licensee?.betLimits ?? []
    ),
    isLoading: isLoadingBrand || isLoadingLicensee,
    error: brandError || licenseeError,
    mutateBrand,
    deleteBrandCache
  }
}

export default useLicenseeBrand
