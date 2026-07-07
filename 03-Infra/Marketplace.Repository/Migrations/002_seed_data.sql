-- Migration 002: Seed inicial (paridade com o antigo InMemoryStore.Seed()).
--   Senhas: todas "123456" hasheadas com BCrypt (workFactor 11).
--   Somente executa INSERT quando o registro não existe (ON CONFLICT DO NOTHING),
--   preservando idempotência entre restarts.

INSERT INTO users (id, email, name, cpf, role, banned, password_hash) VALUES
    ('user-buyer-1',  'comprador@teste.com', 'João Comprador',  '12345678901', 'buyer',  false, '$2a$11$fBtUTzhr4gYhhclVnbx4..vU0M6nRi2QXYVK1chpcJD34h.WWWDzy'),
    ('user-seller-1', 'vendedor@teste.com',  'Maria Vendedora', '98765432100', 'seller', false, '$2a$11$fBtUTzhr4gYhhclVnbx4..vU0M6nRi2QXYVK1chpcJD34h.WWWDzy'),
    ('user-admin-1',  'admin@teste.com',     'Admin Sistema',   '11122233344', 'admin',  false, '$2a$11$fBtUTzhr4gYhhclVnbx4..vU0M6nRi2QXYVK1chpcJD34h.WWWDzy')
ON CONFLICT (id) DO NOTHING;

INSERT INTO seller_profiles (user_id, document_type, document, pix_key, origin_cep, origin_address, onboarding_complete) VALUES
    ('user-seller-1', 'cpf', '98765432100', 'vendedor@teste.com', '01310100', 'Av. Paulista, 1000 - São Paulo/SP', true)
ON CONFLICT (user_id) DO NOTHING;

INSERT INTO categories (id, name, parent_id) VALUES
    ('cat-1', 'Eletrônicos', NULL),
    ('cat-2', 'Celulares',   'cat-1'),
    ('cat-3', 'Moda',        NULL),
    ('cat-4', 'Casa',        NULL)
ON CONFLICT (id) DO NOTHING;

INSERT INTO products (id, title, description, price, stock, category_id, seller_id, seller_name, weight, width, height, length) VALUES
    ('prod-1', 'Smartphone Pro Max',       'Smartphone com tela AMOLED de 6.7 polegadas, 256GB de armazenamento.', 3499.90,  15, 'cat-2', 'user-seller-1', 'Maria Vendedora', 0.200,  8, 16,  1),
    ('prod-2', 'Fone Bluetooth Premium',   'Fone com cancelamento de ruído ativo e bateria de 30h.',                499.90,  30, 'cat-1', 'user-seller-1', 'Maria Vendedora', 0.300, 20, 20,  8),
    ('prod-3', 'Camiseta Básica Algodão',  'Camiseta 100% algodão, disponível em várias cores.',                     79.90, 100, 'cat-3', 'user-seller-1', 'Maria Vendedora', 0.250, 30,  2, 25),
    ('prod-4', 'Luminária de Mesa LED',    'Luminária articulada com 3 níveis de brilho.',                          189.90,  20, 'cat-4', 'user-seller-1', 'Maria Vendedora', 1.200, 15, 40, 15),
    ('prod-5', 'Notebook Ultra Slim',      'Notebook leve com processador de última geração e SSD 512GB.',         5299.00,   8, 'cat-1', 'user-seller-1', 'Maria Vendedora', 1.500, 35,  2, 25),
    ('prod-6', 'Relógio Esportivo',        'Relógio resistente à água com monitor de frequência cardíaca.',         899.00,  12, 'cat-3', 'user-seller-1', 'Maria Vendedora', 0.100,  5,  5,  2)
ON CONFLICT (id) DO NOTHING;

INSERT INTO product_images (product_id, position, url) VALUES
    ('prod-1', 0, 'https://picsum.photos/seed/phone/600/600'),
    ('prod-2', 0, 'https://picsum.photos/seed/headphone/600/600'),
    ('prod-3', 0, 'https://picsum.photos/seed/shirt/600/600'),
    ('prod-4', 0, 'https://picsum.photos/seed/lamp/600/600'),
    ('prod-5', 0, 'https://picsum.photos/seed/laptop/600/600'),
    ('prod-6', 0, 'https://picsum.photos/seed/watch/600/600')
ON CONFLICT (product_id, position) DO NOTHING;

INSERT INTO orders (
    id, buyer_id, buyer_name, product_id, product_title, seller_id, seller_name,
    quantity, product_price, shipping_cost, total, status,
    receipt_url, tracking_code, cep, street, number, complement, neighborhood, city, state,
    created_at, updated_at
) VALUES
    (
        'order-1', 'user-buyer-1', 'João Comprador', 'prod-2', 'Fone Bluetooth Premium', 'user-seller-1', 'Maria Vendedora',
        1, 499.90, 25.50, 525.40, 'Em Análise',
        'https://picsum.photos/seed/receipt/400/600', NULL, '22041080', 'Rua das Flores', '123', NULL, NULL, 'Rio de Janeiro', 'RJ',
        NOW() - INTERVAL '1 day', NOW()
    ),
    (
        'order-2', 'user-buyer-1', 'João Comprador', 'prod-3', 'Camiseta Básica Algodão', 'user-seller-1', 'Maria Vendedora',
        2, 159.80, 18.00, 177.80, 'Enviado',
        'https://picsum.photos/seed/receipt2/400/600', 'BR123456789BR', '22041080', 'Rua das Flores', '123', NULL, NULL, 'Rio de Janeiro', 'RJ',
        NOW() - INTERVAL '2 days', NOW()
    )
ON CONFLICT (id) DO NOTHING;

INSERT INTO repasses (id, order_id, seller_id, seller_name, product_amount, shipping_amount, commission, net_amount, paid, created_at) VALUES
    ('repasse-1', 'order-2', 'user-seller-1', 'Maria Vendedora', 159.80, 18.00, 15.98, 161.82, false, NOW() - INTERVAL '1 day')
ON CONFLICT (id) DO NOTHING;
