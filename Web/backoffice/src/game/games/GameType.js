import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useTrans, Trans } from "../../i18n"
import { HookFormSelect } from "../../select/HookFormSelect"

const Sportsbook = "Sportsbook"
const SportsbookTrans = {
  zh: "体育竞赛",
  en: "Sportsbook"
}

const VirtualSports = "VirtualSports"
const VirtualSportsTrans = {
  zh: "虚拟体育",
  en: "Virtual Sports"
}

const ESports = "ESports"
const ESportsTrans = {
  zh: "电竞体育",
  en: "E-sports"
}

const LiveDealer = "LiveDealer"
const LiveDealerTrans = {
  zh: "真人娱乐",
  en: "Live Dealer"
}

const LiveDealerLobby = "LiveDealerLobby"
const LiveDealerLobbyTrans = {
  zh: "真人视讯大厅",
  en: "Live Dealer Lobby"
}

const CardGame = "CardGame"
const CardGameTrans = {
  zh: "棋牌游戏",
  en: "Card Game"
}

const CardGameLobby = "CardGameLobby"
const CardGameLobbyTrans = {
  zh: "棋牌游戏大厅",
  en: "Card Game Lobby"
}

const Fishing = "Fishing"
const FishingTrans = {
  zh: "捕鱼游戏",
  en: "Fishing Game"
}

const FishingLobby = "FishingLobby"
const FishingLobbyTrans = {
  zh: "捕鱼游戏大厅",
  en: "Fishing Game Lobby"
}

const Arcade = "Arcade"
const ArcadeTrans = {
  zh: "街机游戏",
  en: "Arcade Game"
}

const ArcadeLobby = "ArcadeLobby"
const ArcadeLobbyTrans = {
  zh: "接机游戏大厅",
  en: "Arcade Game Lobby"
}

const Slot = "Slot"
const SlotTrans = {
  zh: "电子游艺",
  en: "Slot Game"
}

const Lottery = "Lottery"
const LotteryTrans = {
  zh: "彩票游戏",
  en: "Lottery"
}

const TableGame = "TableGame"
const TableGameTrans = {
  zh: "桌面游戏",
  en: "Table Game"
}

const GameTypeList = [
  Sportsbook,
  VirtualSports,
  ESports,
  LiveDealer,
  LiveDealerLobby,
  CardGame,
  CardGameLobby,
  Fishing,
  FishingLobby,
  Arcade,
  ArcadeLobby,
  Slot,
  Lottery,
  TableGame
]

const GameTypeTrans = {
  Sportsbook: SportsbookTrans,
  VirtualSports: VirtualSportsTrans,
  ESports: ESportsTrans,
  LiveDealer: LiveDealerTrans,
  LiveDealerLobby: LiveDealerLobbyTrans,
  CardGame: CardGameTrans,
  CardGameLobby: CardGameLobbyTrans,
  Fishing: FishingTrans,
  FishingLobby: FishingLobbyTrans,
  Arcade: ArcadeTrans,
  ArcadeLobby: ArcadeLobbyTrans,
  Slot: SlotTrans,
  Lottery: LotteryTrans,
  TableGame: TableGameTrans
}

export const GameTypeLabel = ({ value }) => {
  switch (value) {
    case Sportsbook:
      return <Trans {...SportsbookTrans} />

    case VirtualSports:
      return <Trans {...VirtualSportsTrans} />

    case ESports:
      return <Trans {...ESportsTrans} />

    case LiveDealer:
      return <Trans {...LiveDealerTrans} />

    case LiveDealerLobby:
      return <Trans {...LiveDealerLobbyTrans} />

    case CardGame:
      return <Trans {...CardGameTrans} />

    case CardGameLobby:
      return <Trans {...CardGameLobbyTrans} />

    case Fishing:
      return <Trans {...FishingTrans} />

    case FishingLobby:
      return <Trans {...FishingLobbyTrans} />

    case Arcade:
      return <Trans {...ArcadeTrans} />

    case ArcadeLobby:
      return <Trans {...ArcadeLobbyTrans} />

    case Slot:
      return <Trans {...SlotTrans} />

    case Lottery:
      return <Trans {...LotteryTrans} />

    case TableGame:
      return <Trans {...TableGameTrans} />

    default:
      return value ?? null
  }
}

export const GameTypeSelect = ({ placeholder, ...props } = {}) => {
  let defaultPlaceholder = useTrans({
    zh: "游戏类型",
    en: "Game Type"
  })
  placeholder = placeholder ?? defaultPlaceholder

  let options = GameTypeList.map((gameType) => ({
    label: useTrans(GameTypeTrans[gameType]),
    value: gameType
  }))
  return (
    <HookFormSelect placeholder={placeholder} options={options} {...props} />
  )
}
