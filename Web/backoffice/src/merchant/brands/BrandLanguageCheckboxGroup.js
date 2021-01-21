import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray } from "react-hook-form"
import { Table, Set, Heading, Text } from "bumbag"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import {
  LanguageCheckboxGroupTableHeadRow,
  LanguageCheckboxGroupTableRow
} from "../../language/LanguageCheckboxGroup"
import { Trans } from "../../i18n"
import Input from "../../input/Input"

export const BrandLanguageCheckboxGroupTable = ({
  name = "brandCultures",
  fieldArrayName = "languages",
  showBrandCultureCode = false,
  width = "100%",
  disabled = false,
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()
  let languages = watch(fieldArrayName)
  const { fields } = useFieldArray({
    control,
    name: fieldArrayName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllPropChecked(selection)

  return (
    <Set orientation="vertical" width={width}>
      <Table width={width}>
        <Table.Head>
          <LanguageCheckboxGroupTableHeadRow
            name={name}
            fields={fields}
            checkedFieldName={checkedFieldName}
            allChecked={allChecked}
            disabled={disabled}
            addonAfter={
              showBrandCultureCode && (
                <Table.HeadCell>
                  <Trans zh="自定代码" en="Brand Culture Code" />
                </Table.HeadCell>
              )
            }
          />
        </Table.Head>
        <Table.Body>
          {fields.map((field) => (
            <LanguageCheckboxGroupTableRow
              key={field._id}
              name={name}
              disabled={disabled}
              field={field}
              checkedFieldName={checkedFieldName}
              addonAfter={
                showBrandCultureCode && (
                  <Table.Cell>
                    <Input
                      name={brandCultureCodeFieldName(name, field)}
                      size="small"
                      disabled={disabled}
                    />
                  </Table.Cell>
                )
              }
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const checkedFieldName = (name, checkboxField) =>
  `${name}.${checkboxField.cultureCode}.checked`

const brandCultureCodeFieldName = (name, checkboxField) => {
  let result = `${name}.${checkboxField.cultureCode}.brandCultureCode`
  console.log("name", result)
  return result
}
