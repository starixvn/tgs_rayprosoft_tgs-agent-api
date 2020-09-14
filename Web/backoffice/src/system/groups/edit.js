import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync } from "react-async"
import client from "../../client"
import { Formik, Field, useFormik } from "formik"
import {
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
//import { roles as fetchRoles } from "../../client/role"
import { getMenus } from "../../client/setting"
import {
  label as systemSubMenuLabel,
  Code as SystemCode,
  name as systemModuleLabel
} from "../index"
import { label as menuLabel, find as findMenu } from "../../menu"

export const AddButton = ({}) => {
  let { data, run: fetchMenus, ...asyncRoles } = useAsync({
    deferFn: getMenus
  })

  let [menus = [], setMenus] = useState(data)

  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked
    setMenus(menus)
  }

  let [promise, setSubmit] = useState(Promise.resolve())
  let async = useAsync({ promise })

  useEffect(() => {
    setMenus(data)
  }, [data])

  //TODO: dialog loading status
  return (
    <Formik
      initialValues={{ name: "" }}
      onSubmit={(form) => {
        let data = {
          ...form,
          menus
        }

        let promise = client.post("/api/role", { data })
        setSubmit(promise)
      }}
    >
      {(formProps) => {
        return (
          <Modal.State>
            <Modal.Disclosure
              use={Button}
              onClick={fetchMenus}
              css={css`
                flex: unset;
              `}
            >
              {add()}
            </Modal.Disclosure>
            <Dialog.Modal
              title={title()}
              showActionButtons
              actionButtonsProps={{
                onClickSubmit: formProps.submitForm,
                onClickCancel: formProps.resetForm
              }}
            >
              <form
                onReset={formProps.handleReset}
                onSubmit={formProps.handleSubmit}
              >
                <div
                  css={css`
                    display: grid;
                    grid-template-rows: auto;
                    grid-template-areas:
                      "name input"
                      "group table";
                  `}
                >
                  <Box
                    css={css`
                      grid-area: name;
                    `}
                    margin="major-1"
                  >
                    <Label
                      htmlFor="name"
                      marginTop="major-1"
                      css={css`
                        line-height: unset;
                      `}
                    >
                      {name()}
                    </Label>
                  </Box>

                  <Box
                    css={css`
                      grid-area: input;
                    `}
                    margin="major-1"
                  >
                    <Field name="name">
                      {({ field, form, meta }) => (
                        <Input id="name" {...field} />
                      )}
                    </Field>
                  </Box>
                  <Box
                    css={css`
                      grid-area: group;
                    `}
                    margin="major-1"
                  >
                    <Label
                      htmlFor="name"
                      marginTop="major-1"
                      css={css`
                        line-height: unset;
                      `}
                    >
                      {control()}
                    </Label>
                  </Box>
                  <Box
                    css={css`
                      grid-area: table;
                    `}
                    margin="major-1"
                  >
                    <MenuTable menus={menus} setChecked={setChecked} />
                  </Box>
                </div>
              </form>
            </Dialog.Modal>
          </Modal.State>
        )
      }}
    </Formik>
  )
}

const MenuTable = ({ menus = [], setChecked }) => {
  return (
    <Table>
      {menus.map((menu, index) => {
        return (
          <MenuTableContent key={index} menu={menu} setChecked={setChecked} />
        )
      })}
    </Table>
  )
}

const MenuTableContent = ({ menu = {}, setChecked }) => {
  let { code: moduleCode, subMenus = [] } = menu
  let label = menuLabel([moduleCode])
  if (label === "") {
    return null
  }

  return (
    <>
      <Table.Head>
        <Table.Row>
          <Table.HeadCell colSpan="2">
            <MenuCheckbox menu={menu} setChecked={setChecked} />
          </Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {subMenus.map((menu) => {
          let { code: subMenuCode, subMenus: subMenuItems = [] } = menu
          return (
            <Table.Row key={subMenuCode}>
              <Table.Cell>
                <MenuCheckbox
                  path={[moduleCode]}
                  menu={menu}
                  setChecked={setChecked}
                />
              </Table.Cell>
              <Table.Cell>
                <div>
                  {subMenuItems.map((subMenuItem, index) => {
                    return (
                      <MenuCheckbox
                        key={index}
                        path={[moduleCode, subMenuCode]}
                        menu={subMenuItem}
                        setChecked={setChecked}
                      />
                    )
                  })}
                </div>
              </Table.Cell>
            </Table.Row>
          )
        })}
      </Table.Body>
    </>
  )
}

const MenuCheckbox = ({ path = [], menu = {}, setChecked }) => {
  let [checked, set] = useState(menu.isDisplay)

  let { code } = menu
  let menuPath = [...path, code]
  let label = menuLabel(menuPath)
  if (!label) {
    return null
  }

  return (
    <Checkbox
      label={label}
      checked={checked}
      onChange={(event) => {
        let isChecked = event.target.checked
        set(isChecked)
        setChecked(menuPath, isChecked)
      }}
      css={css`
        white-space: nowrap;
      `}
    ></Checkbox>
  )
}

export const EditButton = ({}) => {}

//TODO: i18n
const add = () => "新增"

const title = () => "新增权限组"

const name = () => "权限组名称"

const control = () => "权限管理"
