-- Migration 001: Schema inicial do Marketplace MVP
-- Idempotente: pode rodar em banco vazio ou pré-existente.

CREATE TABLE IF NOT EXISTS users (
    id            text PRIMARY KEY,
    email         text NOT NULL UNIQUE,
    name          text NOT NULL,
    cpf           text NOT NULL,
    role          text NOT NULL,
    banned        boolean NOT NULL DEFAULT false,
    password_hash text NOT NULL
);

CREATE TABLE IF NOT EXISTS seller_profiles (
    user_id             text PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    document_type       text NOT NULL,
    document            text NOT NULL,
    pix_key             text NOT NULL,
    origin_cep          text NOT NULL,
    origin_address      text NOT NULL,
    onboarding_complete boolean NOT NULL DEFAULT false
);

CREATE TABLE IF NOT EXISTS categories (
    id        text PRIMARY KEY,
    name      text NOT NULL,
    parent_id text NULL REFERENCES categories(id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_categories_parent_id ON categories(parent_id);

CREATE TABLE IF NOT EXISTS products (
    id           text PRIMARY KEY,
    title        text NOT NULL,
    description  text NOT NULL,
    price        numeric(18,2) NOT NULL,
    stock        integer NOT NULL,
    category_id  text NOT NULL REFERENCES categories(id),
    seller_id    text NOT NULL REFERENCES users(id),
    seller_name  text NOT NULL,
    weight       numeric(10,3) NOT NULL,
    width        numeric(10,2) NOT NULL,
    height       numeric(10,2) NOT NULL,
    length       numeric(10,2) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_products_category_id ON products(category_id);
CREATE INDEX IF NOT EXISTS idx_products_seller_id ON products(seller_id);

CREATE TABLE IF NOT EXISTS product_images (
    product_id text NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    position   integer NOT NULL,
    url        text NOT NULL,
    PRIMARY KEY (product_id, position)
);

CREATE TABLE IF NOT EXISTS orders (
    id             text PRIMARY KEY,
    buyer_id       text NOT NULL REFERENCES users(id),
    buyer_name     text NOT NULL,
    product_id     text NOT NULL REFERENCES products(id),
    product_title  text NOT NULL,
    seller_id      text NOT NULL REFERENCES users(id),
    seller_name    text NOT NULL,
    quantity       integer NOT NULL,
    product_price  numeric(18,2) NOT NULL,
    shipping_cost  numeric(18,2) NOT NULL,
    total          numeric(18,2) NOT NULL,
    status         text NOT NULL,
    receipt_url    text NULL,
    tracking_code  text NULL,
    cep            text NOT NULL,
    street         text NOT NULL,
    number         text NOT NULL,
    complement     text NULL,
    neighborhood   text NULL,
    city           text NOT NULL,
    state          text NOT NULL,
    created_at     timestamptz NOT NULL,
    updated_at     timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_orders_buyer_id ON orders(buyer_id);
CREATE INDEX IF NOT EXISTS idx_orders_seller_id ON orders(seller_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON orders(status);

CREATE TABLE IF NOT EXISTS repasses (
    id              text PRIMARY KEY,
    order_id        text NOT NULL REFERENCES orders(id),
    seller_id       text NOT NULL REFERENCES users(id),
    seller_name     text NOT NULL,
    product_amount  numeric(18,2) NOT NULL,
    shipping_amount numeric(18,2) NOT NULL,
    commission      numeric(18,2) NOT NULL,
    net_amount      numeric(18,2) NOT NULL,
    paid            boolean NOT NULL DEFAULT false,
    created_at      timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_repasses_seller_id ON repasses(seller_id);
CREATE INDEX IF NOT EXISTS idx_repasses_order_id ON repasses(order_id);
