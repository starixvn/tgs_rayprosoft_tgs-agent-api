import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, useModalState, Icon } from "bumbag"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import {
  LanguageCheckboxGroupTableHeadRow,
  LanguageCheckboxGroupTableRow
} from "../../language/LanguageCheckboxGroup"
import { Trans, useTrans } from "../../i18n"
import Input from "../../input/Input"
import Backdrop from "../../dialog/Backdrop"
import FormDialog from "../../form/FormDialog"
import Disclosure from "../../dialog/Disclosure"
import { ConfirmText } from "../../button/ActionButtons"
import { faLanguage } from "@fortawesome/free-solid-svg-icons"

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

  return useMemo(() => {
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
                  <Table.HeadCell textAlign="center">
                    <Trans zh="设定" en="Setting" />
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
                    <Table.Cell textAlign="center">
                      <Set orientation="horizontal" alignX="center">
                        <Controller
                          control={control}
                          name={brandCultureCodeFieldName(name, field)}
                          render={(
                            { onChange, onBlur, value, name: fieldName, ref },
                            { invalid, isTouched, isDirty }
                          ) => {
                            let brandLangCodeDialog = useModalState()
                            return (
                              <>
                                <Disclosure {...brandLangCodeDialog}>
                                  <Icon icon={faLanguage} type="font-awesome" />
                                </Disclosure>
                                {brandLangCodeDialog.visible && (
                                  <LangCodeDialog
                                    value={value}
                                    dialog={brandLangCodeDialog}
                                    onChange={(code) => {
                                      setValue(fieldName, code)
                                    }}
                                  />
                                )}
                              </>
                            )
                          }}
                        />
                      </Set>
                    </Table.Cell>
                  )
                }
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

const LangCodeDialog = ({ dialog, onChange }) => {
  let title = useTrans({
    zh: `品牌语言代码`,
    en: `Brand Culture Code`
  })
  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        onSubmit={({ code }) => {
          onChange(code)
          dialog.hide()
        }}
        submitText={<ConfirmText />}
        dialog={dialog}
        title={title}
        hideBackdrop={true}
        minWidth="max-content"
      >
        <Input name="code" />
      </FormDialog>
    </Backdrop>
  )
}

const checkedFieldName = (name, checkboxField) =>
  `${name}.${checkboxField.cultureCode}.checked`

const brandCultureCodeFieldName = (name, checkboxField) => {
  let result = `${name}.${checkboxField.cultureCode}.brandCultureCode`
  return result
}
