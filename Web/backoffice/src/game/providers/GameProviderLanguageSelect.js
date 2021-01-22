import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useGameProvider } from "./client"
import Select from "../../select/Select"

export const GameProviderLanguageSelect = ({ gameProviderId, ...props }) => {
  let { data, isLoading, error } = useGameProvider(gameProviderId)
  let options =
    data?.languages?.map(({ cultureCode, ...lang }) => ({
      label: cultureCode,
      value: cultureCode
    })) ?? []

  return <Select options={options} isLoading={isLoading} {...props} />
}
