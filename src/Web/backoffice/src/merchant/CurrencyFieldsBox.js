import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import {
  CurrencyCheckboxGroupTable,
  toCheckedFieldName
} from "../currency/CurrencyCheckboxGroup"

const DefaultCurrencyFieldsName = "currencies"

export const CurrencyFieldsBox = ({
  fieldsName = DefaultCurrencyFieldsName,
  checkedFieldName = toCheckedFieldName,
  readonly
}) => {
  let { watch, control } = useFormContext()

  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let currencies = watch(fieldsName)

  if (currencies.length == 0) {
    return null
  }

  return (
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
}

export const toCheckedCurrencyCodes = (currencies = []) => {
  return currencies.filter(({ checked }) => checked).map(({ code }) => code)
}

export const mapCheckedCurrencyCodes = (
  name = DefaultCurrencyFieldsName,
  checkedFieldName,
  getValues,
  currencyFields = []
) => {
  let checkedCurrencyCodes = []
  return currencyFields.reduce((checkedCurrencyCodes, field, index) => {
    let fieldName = checkedFieldName(name, field, index)
    let checked = getValues(fieldName)
    if (checked) {
      checkedCurrencyCodes.push(field.code)
    }

    return checkedCurrencyCodes
  }, checkedCurrencyCodes)
}

export default CurrencyFieldsBox
