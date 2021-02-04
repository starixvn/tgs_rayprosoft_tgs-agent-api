import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Modal,
  Dialog,
  Card,
  Box,
  Input,
  Group,
  Icon,
  Stack,
  FieldStack,
  InputField,
  Text
} from "bumbag"
import { useBrandAnnouncementList } from "../client/brand"

export const AnnouncementDialog = ({ brandCode, currency, dialog }) => {
  let { items = [] } = useBrandAnnouncementList({
    brandCode,
    currency
  })

  return (
    <Dialog.Modal title="Announcements" showCloseButton {...dialog}>
      <Table>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>Subject</Table.HeadCell>
            <Table.HeadCell>Content</Table.HeadCell>
            <Table.HeadCell>Duration</Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {items.map((announcement) => (
            <AnnouncementRow {...announcement} />
          ))}
        </Table.Body>
      </Table>
    </Dialog.Modal>
  )
}

const AnnouncementRow = ({
  subject = "",
  content = "",
  from = "",
  to = ""
}) => {
  return (
    <Table.Row>
      <Table.Cell>{subject}</Table.Cell>
      <Table.Cell>{content}</Table.Cell>
      <Table.Cell>
        {from} - {to}
      </Table.Cell>
    </Table.Row>
  )
}
