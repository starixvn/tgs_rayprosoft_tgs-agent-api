import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { useLabel as useMenuLabel, find as findMenu } from "../../menu"
import { useMenus, useAddRole, useEditRole, useRole } from "./client"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import InputField from "../../input/Field"
import _FormDialog from "../../form/Dialog"
import cloneDeep from "clone-deep"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let {
    menus,
    isLoading: isLoadingMenus,
    error: fetchMenusError,
    mutate
  } = useMenus({
    shouldFetch: dialog.visible
  })

  useEffect(() => {
    if (!dialog.visible) {
      mutate()
    }
  }, [dialog.visible])

  let disclosureRef = useRef()
  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure ref={disclosureRef} {...dialog}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      <AddDialog
        menus={menus}
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
        visible={dialog.visible && !isLoadingMenus}
      />
      {dialog.visible && isLoadingMenus && <LoadingBackdrop visible={true} />}
      {dialog.visible && fetchMenusError && (
        <AlertDialog {...fetchMenusError} onClickClose={dialog.hide} />
      )}
    </>
  )
}

const AddDialog = ({ menus = [], dialog, onSubmitted, finalFocusRef }) => {
  let [menusState, setMenus] = useState()
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useAddRole({
    data: submitData
  })

  useEffect(() => {
    setMenus(cloneDeep(menus))
  }, [JSON.stringify(menus)])

  let title = useTrans({
    zh: "新增权限组",
    en: "Add Role"
  })

  const clearResult = () => {
    setSubmit(null)
  }

  return (
    <>
      <_FormDialog
        initialValues={{
          name: ""
        }}
        onSubmit={(data) => {
          setSubmit({
            menus: menusState,
            ...data
          })
        }}
        isLoading={isLoading}
        result={result}
        error={error}
        onCloseErrorAlert={clearResult}
        onSubmitted={onSubmitted}
        title={title}
        dialog={dialog}
        placement="top"
        finalFocusRef={finalFocusRef}
      >
        <DialogBody menus={menusState} setMenus={setMenus} dialog={dialog} />
      </_FormDialog>
    </>
  )
}

const DialogBody = ({ menus, setMenus, dialog }) => {
  return (
    <FieldValues>
      <Label name="name" required>
        <Trans zh="权限组名称" en="Name" />
      </Label>
      <InputField name="name" />

      <Label>
        <Trans zh="权限管理" en="Role" />
      </Label>
      <MenusBody menus={menus} setMenus={setMenus} />
    </FieldValues>
  )
}

const MenusBody = ({ menus = [], setMenus }) => {
  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked

    setMenus(menus)
  }

  return <MenuTable menus={menus} setChecked={setChecked} />
}

export const EditButton = ({ id, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure ref={disclosureRef} {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <AsyncRoleDialog
          id={id}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

const AsyncRoleDialog = ({ id, dialog, onSubmitted, finalFocusRef }) => {
  let { role, isLoading, isValidating, error, mutate } = useRole({
    id: dialog.visible && id
  })

  if (isLoading) {
    return <LoadingBackdrop visible={true} />
  }

  if (error) {
    return <AlertDialog {...error} onClickClose={dialog.hide} />
  }

  return (
    <EditDialog
      id={id}
      name={role.name}
      menus={role.menus}
      dialog={dialog}
      onSubmitted={onSubmitted}
      finalFocusRef={finalFocusRef}
    />
  )
}

const EditDialog = ({
  id,
  name,
  menus,
  dialog,
  onSubmitted,
  finalFocusRef
}) => {
  let [menusState, setMenus] = useState()
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useEditRole({
    id,
    data: submitData
  })

  useEffect(() => {
    setMenus(cloneDeep(menus))
  }, [menus])

  let title = useTrans({
    zh: "修改权限组",
    en: "Edit Role"
  })

  const clearResult = () => {
    setSubmit(null)
  }

  return (
    <_FormDialog
      initialValues={{
        name
      }}
      onSubmit={(data) => {
        setSubmit({
          menus: menusState,
          ...data
        })
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      placement="top"
      finalFocusRef={finalFocusRef}
    >
      {({ resetForm, ...formProps }) => {
        useEffect(() => {
          resetForm({
            values: {
              name
            }
          })
        }, [name])

        return (
          <DialogBody menus={menusState} setMenus={setMenus} dialog={dialog} />
        )
      }}
    </_FormDialog>
  )
}

const MenuTable = ({ menus = [], setChecked } = {}) => {
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
  let label = useMenuLabel({ codes: [moduleCode] })
  if (!label) {
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

  useEffect(() => {
    set(menu.isDisplay)
  }, [menu.isDisplay])

  let { code } = menu
  let menuPath = [...path, code]
  let label = useMenuLabel({ codes: menuPath })
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
