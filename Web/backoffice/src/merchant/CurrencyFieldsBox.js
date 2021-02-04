import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import { CurrencyCheckboxGroupTable } from "../currency/CurrencyCheckboxGroup"

export const CurrencyFieldsBox = ({ fieldsName = "currencies", readonly }) => {
  let { watch, control } = useFormContext()

  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let currencies = watch(fieldsName)
  return (
    currencies && (
      <>
        <NormalLabel>
          <Trans zh="币种" en="Currency" />
        </NormalLabel>
        <CurrencyCheckboxGroupTable
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
        />
      </>
    )
  )
}

export const toCheckedCurrencyCodes = (currencies = []) => {
  return currencies.filter(({ checked }) => checked).map(({ code }) => code)
}

export default CurrencyFieldsBox
